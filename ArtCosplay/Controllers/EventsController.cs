using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    public class EventsController(
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
                    Message = "No premission to add events"
                });

                if (!ModelState.IsValid) RedirectToAction("AdminPanel", "Home");


                _appDbContext.Events.Add(new Event { CreatorId = user.Id, Title = model.First, Description = model.Second, Location="Минск", CoverImageUrl="/"});
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
                    Message = "No premissions to delete events"
                });

                var eventItem = _appDbContext.Events.FirstOrDefault(x => x.EventId == model.Id);

                if (eventItem == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "Event not found"
                });

                _appDbContext.Events.Remove(eventItem);
                _appDbContext.SaveChanges();

                return Ok(new
                {
                    Status = "success",
                    Message = "Event was deleted"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting event {ex}");
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Server error"
                });
            }
        }
    }
}
