using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Data.DB
{
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string SourceName { get; set; }

        public string Description { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<Post> Posts { get; set; }
    }
}