using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }

        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public int? DiscussionId { get; set; }
        public int? DiscussionReplyId { get; set; }

        public Post? Post { get; set; }
        public Comment? Comment { get; set; }
        public Discussion? Discussion { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}