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

        [HttpGet]
        public IActionResult Comment(int id)
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
        public async Task<IActionResult> Comment([FromBody] CommentViewModel model)
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

                await _appDbContext.Comments.AddAsync(new Comment
                {
                    DiscussionId = model.Id,
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
    }
}
