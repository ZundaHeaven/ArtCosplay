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
        public IActionResult Profile() => View(); 
        public IActionResult ArtPage() => View();
        public IActionResult DiscusPage() => View();
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

                if (!ModelState.IsValid) return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation error"
                });

                if (model.Image.Length > 4096 * 1024) return BadRequest(new
                {
                    Status = "error",
                    Message = "File size is more than 4MB"
                });

                List<string> acceptableExtensions =
                    [".jpg", ".jpeg", ".png", ".webp"];

                string extension = Path.GetExtension(model.Image.FileName);

                if (!(acceptableExtensions.Contains(extension) 
                    && model.Image.ContentType.ToLower().Contains("image"))) return BadRequest(new
                {
                    Status = "error",
                    Message = "File format is not supported"
                });

                string path;
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    byte[] hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(model.Image.FileName + DateTime.Now.ToString()));
                     path = "/Posts/" + Encoding.UTF8.GetString(hash) + extension;
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
                _logger.LogError(ex.Message);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
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
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
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
