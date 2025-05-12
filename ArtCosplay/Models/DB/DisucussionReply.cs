using ArtCosplay.Models.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DiscussionReply
{
    [Key]
    public int ReplyId { get; set; }

    [ForeignKey("Discussion")]
    public int DiscussionId { get; set; }
    public Discussion Discussion { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}