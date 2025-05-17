using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(2000)]
        public string Content { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}