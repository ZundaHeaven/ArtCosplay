using ArtCosplay.Models.DB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

public class Post
{
    [Key]
    public int PostId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("User")]
    public int AuthorId { get; set; }
    public User Author { get; set; }

    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<PostCharacter> PostCharacters { get; set; }
}