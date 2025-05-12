using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ArtCosplay.Models.DB
{

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50), Index(IsUnique = true)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(256)]
        public string PasswordHash { get; set; }

        [MaxLength(255)]
        public string AvatarUrl { get; set; }

        [MaxLength(100)]
        public string Bio { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public bool IsArtist { get; set; }
        public bool IsCosplayer { get; set; }
        public bool IsSeller { get; set; }

        // Навигационные свойства
        public ICollection<Post> Posts { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Discussion> Discussions { get; set; }
        public ICollection<DiscussionReply> DiscussionReplies { get; set; }
    }
}
