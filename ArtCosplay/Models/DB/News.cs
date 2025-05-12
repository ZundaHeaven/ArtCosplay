using ArtCosplay.Models.DB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
public class News
{
    [Key]
    public int NewsId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("User")]
    public int AuthorId { get; set; }
    public User Author { get; set; }
}