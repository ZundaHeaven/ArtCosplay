using ArtCosplay.Models;
using ArtCosplay.Models.DB;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        public ActionResult<string> CreateUser([FromBody] RegisterUser user)
        {
            try
            {
                var users = _appDbContext.Users.Where(x => x.Username == user.Name || x.Email == user.Email);

                if(users.Any())
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "An account with this email or username already exists in our system"
                    });
                }

                var results = new List<ValidationResult>();
                var context = new ValidationContext(user);
                if (!Validator.TryValidateObject(user, context, results, true))
                {
                    return BadRequest(new
                    {
                        Message = "Several validation errors appeared",
                        ValidationErrors = results.Select(x => x.ErrorMessage)
                    });
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: user.Password,
                    salt: Encoding.UTF8.GetBytes("ExampleSalt"),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8)
                );


                _appDbContext.Users.Add(new User
                {
                    Username = user.Name,
                    Email = user.Email,
                    PasswordHash = hashed,
                    Bio = user.About,
                    AvatarUrl = $"/data/placeholder.jpg"
                });
                _appDbContext.SaveChanges();

                return StatusCode(201, new
                {
                    Status = "success",
                    Message = "User added"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "error",
                    Message = ex.Message
                });
                
            }
        }
    }
}

