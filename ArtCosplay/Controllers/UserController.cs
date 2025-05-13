using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    public class UserController(ILogger<HomeController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                var users = new
                {
                    Status = "success",
                    Users = _appDbContext.Users.Select(x => new
                    {
                        x.Id,
                        x.UserName,
                        x.AvatarUrl,
                        x.Bio,
                        x.IsCosplayer,
                        x.IsArtist,
                        x.IsSeller,
                        x.LastLogin,
                        x.RegistrationDate
                    })
                };

                _logger.LogInformation("Users was fetched");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while fetching users");
                return NotFound(new { 
                    Status = "error",
                    ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Login complete!");
                     return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogInformation("Login fail!");
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Registration() => View();

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = _appDbContext.Users.Where(x => x.UserName == model.Name || x.Email == model.Email);

                if(users.Any())
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким именем или email уже существует!");
                    return View(model);
                } 

                User user = new User { 
                    Email = model.Email, 
                    UserName = model.Name,
                    Bio = model.About,
                    AvatarUrl = $"/data/placeholder.jpg"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("New user was created");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

