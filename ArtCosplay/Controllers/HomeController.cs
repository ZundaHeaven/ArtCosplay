using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace ArtCosplay.Controllers
{
    public class HomeController(ILogger<HomeController> logger, 
        AppDbContext appDbContext, 
        UserManager<User> userManager, 
        IWebHostEnvironment appEnvironment) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _appEnvironment = appEnvironment;

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public async Task<IActionResult> Profile(string? id, int? page)
        {
            try
            {
                if (id == null)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null) return BadRequest(new
                    {
                        Status = "error",
                        Message = "User is null"
                    });

                    id = user.Id;
                }
                else
                {
                    if(_appDbContext.Users.FirstOrDefault(x => x.Id == id) == null)
                    {
                        return NotFound();
                    }
                }

                ViewData["Page"] = page == null ? 1 : page;
                ViewData["Id"] = id;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.Source + ex.StackTrace);
                return NotFound();
            }
        } 
        public IActionResult ArtPage(ArtPageFindViewModel model) => View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(model ?? new ArtPageFindViewModel(), new CreatePostViewModel()));
        public IActionResult DiscusPage(int? page, string? filter)
        {
            ViewData["Filter"] = filter;
            ViewData["Page"] = page ?? 1;
            return View();
        }
        public IActionResult ShopPage() => View();
        public IActionResult CharactersPage() => View();
        public IActionResult Publication() => View();
        public IActionResult ShoppingItem() => View();
        public IActionResult About() => View();
        public IActionResult FAQ() => View();
        public IActionResult Rules() => View();
        public IActionResult ProfileChange() => View();

        [HttpPost]
        public async Task<IActionResult> ArtPage(CreatePostViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Неверная валидация!");
                    return View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(new ArtPageFindViewModel(), model));
                }

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Неверная валидация!");
                    return View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(new ArtPageFindViewModel(), model));
                }

                if (model.Image.Length > 4096 * 1024) {
                    ModelState.AddModelError(string.Empty, "Размер файла не должен превышать 4 мб!");
                    return View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(new ArtPageFindViewModel(), model));
                }


                List<string> acceptableExtensions =
                    [".jpg", ".jpeg", ".png", ".webp"];

                string extension = Path.GetExtension(model.Image.FileName);

                if (!(acceptableExtensions.Contains(extension) 
                    && model.Image.ContentType.ToLower().Contains("image")))
                {
                    ModelState.AddModelError(string.Empty, "Формат файла не поддерживатся!");
                    return View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(new ArtPageFindViewModel(), model));
                }


                string path;
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    byte[] hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(model.Image.FileName + DateTime.Now.ToString()));
                    path = "/data/Posts/" + new string(BitConverter.ToString(hash).Where(x => x != '-').ToArray()) + extension;
                }

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                var entity = _appDbContext.Posts.Add(new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    ImageUrl = path,
                    Type = model.Type,
                    AuthorId = user.Id
                }).Entity;

                _appDbContext.SaveChanges();

                return RedirectToAction("Post", "Home", new { id = entity.PostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.Source + ex.StackTrace);
                
                ModelState.AddModelError(string.Empty, "Ошибка сервера");
                return View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(new ArtPageFindViewModel(), model));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DiscusPage(CreateDiscusViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Неверная валидация!");
                    return View(model);
                }

                Discussion discussion = new Discussion
                {
                    AuthorId = user.Id,
                    Title = model.Title,
                    Content = model.Content
                };

                var entity = _appDbContext.Add(discussion).Entity;
                _appDbContext.SaveChanges();
                _logger.LogInformation($"New discussion was added with id: {entity.DiscussionId} by {entity.AuthorId}");
                return RedirectToAction("Discussion", "Home", new { id = entity.DiscussionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError(string.Empty, "Ошибка сервера");
                return View(model);
            }
        }

        public IActionResult Post(int id)
        {
            ViewData["PostId"] = id;
            return View();
        }

        public IActionResult Discussion(int id)
        {
            ViewData["DiscussionId"] = id;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
