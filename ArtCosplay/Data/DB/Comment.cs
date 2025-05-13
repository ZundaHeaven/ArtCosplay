using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string Text { get; set; }

        public int? PostId { get; set; }
        public int? DiscussionId { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }

        [ForeignKey("DiscussionId")]
        public Discussion? Discussion { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Like> Likes { get; set; }
    }
}