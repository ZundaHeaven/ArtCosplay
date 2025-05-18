using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Data.DB
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(20)]
        public override string UserName { get; set; } 

        [Required]
        [MaxLength(100)]
        public override string Email { get; set; }   

        [MaxLength(255)]
        public string AvatarUrl { get; set; }

        [MaxLength(100)]
        public string Bio { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public bool IsArtist { get; set; }
        public bool IsCosplayer { get; set; }
        public bool IsSeller { get; set; }

        public ICollection<Event> Events { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Discussion> Discussions { get; set; }
    }
}
