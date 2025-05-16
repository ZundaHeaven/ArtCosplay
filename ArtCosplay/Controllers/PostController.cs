using ArtCosplay.Data.DB;
using ArtCosplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using ArtCosplay.Models;

namespace ArtCosplay.Controllers
{
    public class PostController(ILogger<PostController> logger, AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly ILogger<PostController> _logger = logger;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpGet]
        public IActionResult Get(int? id)
        {
            try
            {
                var posts = _appDbContext.Posts
                        .Include(x => x.Likes)
                        .Include(x => x.Comments);

                if (id != null)
                {
                    posts = posts.Where(x => x.PostId == id);
                }

                return Ok(new
                {
                    Status = "success",
                    Message = "Posts was fetched",
                    Posts = posts.Select(x => new
                    {
                        Id = x.PostId,
                        AuthorId = x.AuthorId,
                        Title = x.Title,
                        Content = x.Content,
                        LikesCount = x.Likes.Count,
                        CommentsCount = x.Comments.Count
                    })
                });
            }
            catch(Exception e)
            {
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
                var posts = _appDbContext.Posts
                    .FirstOrDefault(x => x.PostId == id);
                if (posts == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No post was founded"
                });

                var likes = _appDbContext.Likes
                    .Where(x => x.PostId == id);

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
        public async Task<IActionResult> Like([FromBody] IdModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var post = _appDbContext.Posts
                    .FirstOrDefault(x => x.PostId == model.Id);
                if (post == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No post was founded"
                });

                var like = _appDbContext.Likes
                    .FirstOrDefault(x => x.PostId == model.Id && x.UserId == user.Id);

                if (like == null)
                {
                    _appDbContext.Add(new Like
                    {
                        UserId = user.Id,
                        PostId = model.Id
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
                    LikesCount = _appDbContext.Likes.Where(x => x.PostId == model.Id).ToList().Count
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

                var post = _appDbContext.Posts
                    .FirstOrDefault(x => x.PostId == id);
                if (post == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No post was founded"
                });

                if(!(await _userManager.IsInRoleAsync(user, "Admin") 
                    || await _userManager.IsInRoleAsync(user, "Moderator")
                    || post.AuthorId == user.Id))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "No premission to delete "
                    });
                }

                _appDbContext.Posts.Remove(post);
                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Post was deleted"
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
