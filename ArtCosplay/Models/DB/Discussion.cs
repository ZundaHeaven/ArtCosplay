using ArtCosplay.Models.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Discussion
{
    [Key]
    public int DiscussionId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("User")]
    public int AuthorId { get; set; }
    public User Author { get; set; }

    public ICollection<DiscussionReply> Replies { get; set; }
}