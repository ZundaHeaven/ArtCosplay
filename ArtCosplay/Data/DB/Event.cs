using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(255)]
        public string CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public string CreatorId { get; set; }
        public User Creator { get; set; }
    }
}