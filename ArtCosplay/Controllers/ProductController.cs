using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtCosplay.Controllers
{
    public class ProductController(
        ILogger<PostController> logger,
        AppDbContext appDbContext,
        UserManager<User> userManager,
        IWebHostEnvironment appEnvironment) : Controller
    {
        private readonly ILogger<PostController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _appEnvironment = appEnvironment;

        [HttpGet]
        public IActionResult Get(int? id)
        {
            try
            {
                var posts = _appDbContext.Products.ToList();

                if (id != null)
                {
                    posts = posts.Where(x => x.ProductId == id).ToList();
                }

                return Ok(new
                {
                    Status = "success",
                    Message = "Products was fetched",
                    Posts = posts.Select(x => new
                    {
                        Id = x.ProductId,
                        AuthorId = x.SellerId,
                        Title = x.Title,
                        Description = x.Description,
                        City = x.City,
                        Type = x.Type,
                        Price = x.Price
                    })
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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] IdModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var item = _appDbContext.Products
                    .FirstOrDefault(x => x.ProductId == model.Id);
                if (item == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No product was founded"
                });

                if (!(await _userManager.IsInRoleAsync(user, "Admin")
                    || await _userManager.IsInRoleAsync(user, "Moderator")
                    || item.SellerId == user.Id))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "No premission to deactivate "
                    });
                }

                item.IsAvailable = !item.IsAvailable;

                _appDbContext.Products.Update(item);
                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Product was deactivated"
                });
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

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] IdModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var item = _appDbContext.Products
                    .FirstOrDefault(x => x.ProductId == model.Id);
                if (item == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No product was founded"
                });

                if (!(await _userManager.IsInRoleAsync(user, "Admin")
                    || await _userManager.IsInRoleAsync(user, "Moderator")
                    || item.SellerId == user.Id))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "No premission to delete "
                    });
                }

                _appDbContext.Products.Remove(item);
                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Product was deactivated"
                });
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
    }
}
