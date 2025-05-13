using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
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
        public string AuthorId { get; set; }
        public User Author { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; } 
    }
}