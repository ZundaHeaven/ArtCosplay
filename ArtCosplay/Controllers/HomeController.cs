using System.Diagnostics;
using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    public class HomeController(ILogger<HomeController> logger, AppDbContext appDbContext) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult Registration() => View();
        public IActionResult Login() => View();
        public IActionResult Profile() => View(); 
        public IActionResult ArtPage() => View();
        public IActionResult DiscusPage() => View();
        public IActionResult ShopPage() => View();
        public IActionResult CharactersPage() => View();
        public IActionResult Publication() => View();
        public IActionResult ShoppingItem() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
