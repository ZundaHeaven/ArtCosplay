using ArtCosplay.Models.DB;
using Microsoft.AspNetCore.Mvc;

namespace ArtCosplay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;

        public UserController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                return Ok(_appDbContext.Users.Select(x =>
                new {
                    x.UserId,
                    x.Username,
                    x.AvatarUrl,
                    x.Bio,
                    x.IsCosplayer,
                    x.IsArtist,
                    x.IsSeller,
                    x.LastLogin,
                    x.RegistrationDate
                }));
            }
            catch (Exception ex)
            {
                return NotFound("User not found");
            }
        }

        [HttpPost]
        public ActionResult<string> CreateUser(User User)
        {
            try
            {
                _appDbContext.Users.Add(User);
                _appDbContext.SaveChanges();
                return StatusCode(201, "User added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

