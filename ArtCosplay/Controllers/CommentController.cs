using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using ArtCosplay.Models;

namespace ArtCosplay.Controllers
{
    public class CommentController(ILogger<HomeController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpPost]
        public async Task<IActionResult> CommentLike([FromBody] int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var comment = _appDbContext.Comments
                    .FirstOrDefault(x => x.CommentId == id);
                if (comment == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No comment was founded"
                });

                var like = await _appDbContext.Likes
                    .FirstOrDefaultAsync(x => x.CommentId == id && x.UserId == user.Id);

                if (like == null)
                {
                    await _appDbContext.AddAsync(new Like
                    {
                        UserId = user.Id,
                        CommentId = id
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

        [HttpGet]
        public IActionResult CommentPost(int id)
        {
            try
            {
                var post = _appDbContext.Posts
                    .FirstOrDefault(x => x.PostId == id);
                if (post == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No post was founded"
                });

                var comments = _appDbContext.Comments
                    .Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.PostId == post.PostId);

                return Ok(new
                {
                    Status = "success",
                    Message = "Comments was fetched",
                    Comments = comments.Select(x =>
                        new
                        {
                            Id = x.CommentId,
                            UserId = x.UserId,
                            CreatedAt = x.CreatedAt,
                            LikesCount = x.Likes.Count,
                            Content = x.Text
                        }
                    )
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
        public async Task<IActionResult> CommentPost([FromBody] CommentViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var post = _appDbContext.Posts.FirstOrDefault(x => x.PostId == model.Id);
                if (post == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No post was founded"
                });

                if (!ModelState.IsValid) return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation error"
                });

                await _appDbContext.Comments.AddAsync(new Comment
                {
                    PostId = model.Id,
                    UserId = user.Id,
                    Text = model.Content
                });

                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Comment was added!"
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
