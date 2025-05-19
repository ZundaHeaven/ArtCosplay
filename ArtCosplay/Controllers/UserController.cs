using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    public class UserController(ILogger<UserController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<UserController> _logger = logger;
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
                _logger.LogError($"Error while fetching users {ex}");
                return StatusCode(500, new
                {
                    Status = "Server error",
                    ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                return RedirectToAction("Index", "Home");

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
        public async Task<IActionResult> Registration()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = _appDbContext.Users.Where(x => x.UserName == model.Name || x.Email == model.Email);

                if (users.Any())
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким именем или email уже существует!");
                    return View(model);
                }

                User user = new User
                {
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

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest(new
            {
                Status = "error",
                Message = "User not found"
            });

            return Ok(new
            {
                Status = "success",
                Message = "User data was fetched!",
                User = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    AvatarUrl = user.AvatarUrl,
                    user.LastLogin,
                    user.Bio
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Role([FromBody] AddRoleViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.userId);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User not found"
                });

                var sender = await _userManager.GetUserAsync(User);

                if (sender == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "Not authorized"
                });

                if (!await _userManager.IsInRoleAsync(sender, "Admin")) return BadRequest(new
                {
                    Status = "error",
                    Message = "No premissions to add role to the user"
                });

                var result = await _userManager.AddToRoleAsync(user, model.roleName);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        Status = "success",
                        Message = "User added to role successfully"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Error while adding role",
                        Errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching users {ex}");
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
            }
        }
    }
}

