using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using ArtCosplay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace ArtCosplay.Controllers
{
    public class DiscussionController(ILogger<HomeController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet]
        public IActionResult Get(int? id)
        {
            try
            {
                var discussions = _appDbContext.Discussions
                        .Include(x => x.Likes)
                        .Include(x => x.Comments);

                if (id != null)
                    discussions = discussions.Where(x => x.DiscussionId == id);

                return Ok(new
                {
                    Status = "success",
                    Message = "Discussions was fetched",
                    Disucssions = discussions
                        .Select(x => new
                        {
                            Id = x.DiscussionId,
                            AuthorId = x.AuthorId,
                            Title = x.Title,
                            Content = x.Content,
                            LikesCount = x.Likes.Count,
                            CommentsCount = x.Comments.Count,
                            CreatedAt = x.CreatedAt,
                        })
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

        [HttpGet]
        public IActionResult Like(int? id)
        {
            try
            {
                var discussion = _appDbContext.Discussions
                    .FirstOrDefault(x => x.DiscussionId == id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });

                var likes = _appDbContext.Likes
                    .Where(x => x.DiscussionId == id);

                return Ok(new
                {
                    Status = "success",
                    Message = "Likes was fetched",
                    Likes = likes.Select(x => new
                        {
                            Id = x.LikeId,
                            AuthorId = x.UserId,
                            CreatedAt = x.CreatedAt
                        })
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

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var discussion = _appDbContext.Discussions
                    .FirstOrDefault(x => x.DiscussionId == id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });

                var like = await _appDbContext.Likes
                    .FirstOrDefaultAsync(x => x.DiscussionId == id && x.UserId == user.Id);

                if(like == null)
                {
                    await _appDbContext.AddAsync(new Like
                    {
                        UserId = user.Id,
                        DiscussionId = id
                    });
                }
                else
                {
                    _appDbContext.Remove(like);
                }

                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Like was " + like == null ? "added" : "removed"
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
        public async Task<IActionResult> Delete([FromBody] int? id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });
        
                var discussion = _appDbContext.Discussions
                    .FirstOrDefault(x => x.DiscussionId == id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });
        
                if(!(await _userManager.IsInRoleAsync(user, "Admin") 
                    || await _userManager.IsInRoleAsync(user, "Admin")
                    || discussion.AuthorId == user.Id))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "No premission to delete "
                    });
                }
        
                _appDbContext.Discussions.Remove(discussion);
                await _appDbContext.SaveChangesAsync();
        
                return Ok(new
                {
                    Status = "success",
                    Message = "Discussion was deleted"
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
