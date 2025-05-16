using System.Diagnostics;
using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    public class HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult Profile() => View(); 
        public IActionResult ArtPage(ArtPageFindViewModel model) => View(model);
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
        public async Task<IActionResult> DiscusPage(CreateDiscusViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

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
