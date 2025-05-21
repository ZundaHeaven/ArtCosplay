using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtCosplay.Models;

namespace ArtCosplay.Controllers
{
    public class NewsController(
        ILogger<PostController> logger,
        AppDbContext appDbContext,
        UserManager<User> userManager,
        IWebHostEnvironment appEnvironment) : Controller
    {
        private readonly ILogger<PostController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _appEnvironment = appEnvironment;

        [HttpPost]
        public async Task<IActionResult> Add(AddTextInfoViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "erroe",
                    Message = "User is null"
                });

                if (!await _userManager.IsInRoleAsync(user, "Admin")) return BadRequest(new
                {
                    Status = "erroe",
                    Message = "No premission to add news"
                });

                if (!ModelState.IsValid) RedirectToAction("AdminPanel", "Home");

               
                _appDbContext.News.Add(new News { AuthorId = user.Id, Content = model.First, Title = model.Second, ImageUrl="/" });
                _appDbContext.SaveChanges();

                return RedirectToAction("AdminPanel", "Home");
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] IdModel<int> model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                if (!await _userManager.IsInRoleAsync(user, "Admin")) return BadRequest(new
                {
                    Status = "error",
                    Message = "No premissions to delete news"
                });

                var news = _appDbContext.News.FirstOrDefault(x => x.NewsId == model.Id);

                if (news == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "News not found"
                });

                _appDbContext.News.Remove(news);
                _appDbContext.SaveChanges();

                return Ok(new
                {
                    Status = "success",
                    Message = "News was deleted"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting news {ex}");
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
            }
        }
    }
}
