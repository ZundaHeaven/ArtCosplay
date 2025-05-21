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

                var comment = _appDbContext.Comments
                    .FirstOrDefault(x => x.CommentId == model.Id);
                if (comment == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No comment was founded"
                });

                if (!(await _userManager.IsInRoleAsync(user, "Admin")
                    || await _userManager.IsInRoleAsync(user, "Moderator")
                    || comment.UserId == user.Id))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "No premission to delete "
                    });
                }

                _appDbContext.Remove(comment);
                _appDbContext.SaveChanges();

                return Ok(new
                {
                    Status = "success",
                    Message = "Comment was deleted"
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
                var comment = _appDbContext.Comments
                    .FirstOrDefault(x => x.CommentId == model.Id);
                if (comment == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No comment was founded"
                });

                var like = _appDbContext.Likes
                    .FirstOrDefault(x => x.CommentId == model.Id && x.UserId == user.Id);

                if (like == null)
                {
                    _appDbContext.Add(new Like
                    {
                        UserId = user.Id,
                        CommentId = model.Id
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
                    LikesCount = _appDbContext.Likes.Where(x => x.CommentId == model.Id).ToList().Count
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
        public IActionResult Post(int id)
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
        public async Task<IActionResult> Post([FromBody] CommentViewModel model)
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

                var comment = (await _appDbContext.Comments.AddAsync(new Comment
                {
                    PostId = model.Id,
                    UserId = user.Id,
                    Text = model.Content
                })).Entity;

                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Comment was added!",
                    CommentId = comment.CommentId
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
        public IActionResult Discussion(int id)
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

                var comments = _appDbContext.Comments
                    .Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.DiscussionId == discussion.DiscussionId);

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
        public async Task<IActionResult> Discussion([FromBody] CommentViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "User is null"
                });

                var discussion = _appDbContext.Discussions.FirstOrDefault(x => x.DiscussionId == model.Id);
                if (discussion == null) return BadRequest(new
                {
                    Status = "error",
                    Message = "No discussion was founded"
                });

                if (!ModelState.IsValid) return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation error"
                });

                var comment = (await _appDbContext.Comments.AddAsync(new Comment
                {
                    DiscussionId = model.Id,
                    UserId = user.Id,
                    Text = model.Content
                })).Entity;

                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = "success",
                    Message = "Comment was added!",
                    CommentId = comment.CommentId
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
