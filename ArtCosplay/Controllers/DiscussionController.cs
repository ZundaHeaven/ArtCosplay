using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using ArtCosplay.Models;

namespace ArtCosplay.Controllers
{
    public class DiscussionController(ILogger<DiscussionController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<DiscussionController> _logger = logger;
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
        public async Task<IActionResult> Like([FromBody] IdModel<int> model)
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
                    .FirstOrDefault(x => x.DiscussionId == model.Id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });

                var like = _appDbContext.Likes
                    .FirstOrDefault(x => x.DiscussionId == model.Id && x.UserId == user.Id);

                if(like == null)
                {
                    _appDbContext.Add(new Like
                    {
                        UserId = user.Id,
                        DiscussionId = model.Id
                    });
                }
                else
                {
                    _appDbContext.Remove(like);
                }

                _appDbContext.SaveChanges();

                return Ok(new
                {
                    Status = "success",
                    Message = "Like was " + like == null ? "added" : "removed",
                    LikesCount = _appDbContext.Likes.Where(x => x.DiscussionId == model.Id).ToList().Count
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
        
                var discussion = _appDbContext.Discussions
                    .FirstOrDefault(x => x.DiscussionId == model.Id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });
        
                if(!(await _userManager.IsInRoleAsync(user, "Admin") 
                    || await _userManager.IsInRoleAsync(user, "Moderator")
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
