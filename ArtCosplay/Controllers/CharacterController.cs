using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtCosplay.Models;
using System.IO;

namespace ArtCosplay.Controllers
{
    public class CharacterController(ILogger<HomeController> logger,
        AppDbContext appDbContext,
        UserManager<User> userManager,
        IWebHostEnvironment appEnvironment) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _appEnvironment = appEnvironment;

        [HttpPost]
        public async Task<IActionResult> Add(AddCharacterViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) return BadRequest(new
                {
                    Status = "erroe",
                    Message = "User is null"
                });

                if(!await _userManager.IsInRoleAsync(user, "Admin")) return BadRequest(new
                {
                    Status = "erroe",
                    Message = "No premission to add character"
                });

                if(!ModelState.IsValid) RedirectToAction("AdminPanel", "Home");

                string extension = Path.GetExtension(model.Image.FileName);

                if (model.Image.Length > 1024 * 4096) RedirectToAction("AdminPanel", "Home");

                string path = $"/data/Characters/{HashedPathGenerator.GeneratePath(model.Image)}";

                using (var fileStream = new FileStream($"{_appEnvironment.WebRootPath}{path}", FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                _appDbContext.Characters.Add(new Character { Description = model.Description, Name = model.Name, SourceName = model.Type, ImageUrl = path });
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
        public async Task<IActionResult> Delete([FromBody]IdModel<int> model)
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
                    Message = "No premission to delete character"
                });

                var character = _appDbContext.Characters.FirstOrDefault(x => x.CharacterId == model.Id);

                if(character == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "Character not found"
                });

                try
                {
                    System.IO.File.Delete($"{_appEnvironment.WebRootPath}{character.ImageUrl}");
                }
                catch { }

                _appDbContext.Characters.Remove(character);
                _appDbContext.SaveChanges();

                return Ok(new
                {
                    Status = "success",
                    Message = "Character was deleted"
                });
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
    }
}
