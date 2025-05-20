using System.Data.Entity;
using System.Diagnostics;
using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Chat(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Registration", "User");

            if (id == null)
                return NotFound();

            var chat = _appDbContext.Chats.FirstOrDefault(x => x.ChatId == id);

            if(chat == null)
                return NotFound();

            if (chat.SellerId != user.Id && chat.BuyerId != user.Id)
                return NotFound();

            ViewData["Id"] = id;

            return View();
        }
        public IActionResult ArtPage(ArtPageFindViewModel? model) => View(new Tuple<ArtPageFindViewModel, CreatePostViewModel>(model ?? new ArtPageFindViewModel(), new CreatePostViewModel()));
        public IActionResult ShopPage(ShopPageFindViewModel? model) => View(new Tuple<ShopPageFindViewModel, CreateShoppingItemViewModel>(model ?? new ShopPageFindViewModel(), new CreateShoppingItemViewModel()));
        public IActionResult CharactersPage(CharacterPageViewModel? model) => View(model ?? new CharacterPageViewModel());
        public IActionResult Character(int id, int? page)
        {
            if (_appDbContext.Characters.FirstOrDefault(x => x.CharacterId == id) == null)
                return NotFound();

            ViewData["Id"] = id;
            ViewData["Page"] = page ?? 1;

            return View();
        }
        public IActionResult Publication() => View();
        public IActionResult ShoppingItem(int? id)
        {
            if (id == null)
                return NotFound();

            var item = _appDbContext.Products.FirstOrDefault(x => x.ProductId == id);

            if(item == null)
                return NotFound();

            ViewData["Id"] = id;

            return View();
        }
        public IActionResult About() => View();
        public IActionResult FAQ() => View();
        public IActionResult Rules() => View();
        public async Task<IActionResult> ProfileEdit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Index");

            return View(new EditUserViewModel
            {
                Bio = user.Bio
            });
        }

        [HttpPost]
        public async Task<IActionResult> ProfileEdit(EditUserViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Неверная валидация!");
                    return View(model);
                }

                user.Bio = model.Bio;

                if(model.OldPassword != null)
                {
                    if (model.NewPassword == null)
                    {
                        ModelState.AddModelError(string.Empty, "Чтобы изменить пароль, сначала заполните значение!");
                        return View(model);
                    }

                    var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
                    
                    if (result != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Пароли не совпадают!");
                        return View(model);
                    }

                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
                }

                if (model.Avatar != null)
                {
                    List<string> acceptableExtensions =
                        [".jpg", ".jpeg", ".png", ".webp"];

                    string extension = Path.GetExtension(model.Avatar.FileName);

                    if (model.Avatar.Length > 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "Размер файла не должен превышать 1 мб!");
                        return View(model);
                    }

                    if (!(acceptableExtensions.Contains(extension)
                        && model.Avatar.ContentType.ToLower().Contains("image")))
                    {
                        ModelState.AddModelError(string.Empty, "Формат файла не поддерживатся!");
                        return View(model);
                    }

                    string path = $"/data/Avatars/{HashedPathGenerator.GeneratePath(model.Avatar)}";

                    Console.WriteLine($"LOOOOOOOOOOOL PATH {path}");

                    using (var fileStream = new FileStream($"{_appEnvironment.WebRootPath}{path}", FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(fileStream);
                    }

                    user.AvatarUrl = path;
                }

                _appDbContext.Users.Update(user);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.Source + ex.StackTrace);

                ModelState.AddModelError(string.Empty, "Ошибка сервера");
                return View(model);
            }
        }

        public IActionResult DiscusPage(int? page, string? filter)
        {
            ViewData["Filter"] = filter;
            ViewData["Page"] = page ?? 1;
            return View();
        }

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
                    if (_appDbContext.Users.FirstOrDefault(x => x.Id == id) == null)
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

                if(_appDbContext.Characters.FirstOrDefault(x=> x.CharacterId == model.CharacterId) == null && model.CharacterId != 0)
                {
                    ModelState.AddModelError(string.Empty, "Нет такого персонажа!");
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

                string path = $"/data/Posts/{HashedPathGenerator.GeneratePath(model.Image)}";

                using (var fileStream = new FileStream($"{_appEnvironment.WebRootPath}{path}", FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                var entity = _appDbContext.Posts.Add(new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    ImageUrl = path,
                    Type = model.Type,
                    AuthorId = user.Id,
                    CharacterId = model.CharacterId == 0 ? null : model.CharacterId
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

        [HttpPost]
        public async Task<IActionResult> ShopPage(CreateShoppingItemViewModel model)
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
                    return View(new Tuple<ShopPageFindViewModel, CreateShoppingItemViewModel>(new ShopPageFindViewModel(), model));
                }

                if (model.Image.Length > 4096 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "Размер файла не должен превышать 4 мб!");
                    return View(new Tuple<ShopPageFindViewModel, CreateShoppingItemViewModel>(new ShopPageFindViewModel(), model));
                }

                List<string> acceptableExtensions =
                    [".jpg", ".jpeg", ".png", ".webp"];

                string extension = Path.GetExtension(model.Image.FileName);

                if (!(acceptableExtensions.Contains(extension)
                    && model.Image.ContentType.ToLower().Contains("image")))
                {
                    ModelState.AddModelError(string.Empty, "Формат файла не поддерживатся!");
                    return View(new Tuple<ShopPageFindViewModel, CreateShoppingItemViewModel>(new ShopPageFindViewModel(), model));
                }

                string path = $"/data/Products/{HashedPathGenerator.GeneratePath(model.Image)}";

                using (var fileStream = new FileStream($"{_appEnvironment.WebRootPath}{path}", FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                Console.WriteLine("FDSFSDFDSFSDFSDFSDF" + model.Price);

                var entity = _appDbContext.Products.Add(new Product
                {
                    Title = model.Title,
                    Description = model.Content,
                    ImageUrl = path,
                    Type = model.Type,
                    SellerId = user.Id,
                    City = model.City,
                    Price = Convert.ToInt32(model.Price),
                    IsAvailable = true
                }).Entity;

                _appDbContext.SaveChanges();

                return RedirectToAction("ShoppingItem", "Home", new { id = entity.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError(string.Empty, "Ошибка сервера");
                return View(new Tuple<ShopPageFindViewModel, CreateShoppingItemViewModel>(new ShopPageFindViewModel(), model));
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
