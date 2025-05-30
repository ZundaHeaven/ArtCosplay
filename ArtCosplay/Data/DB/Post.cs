﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("User")]
        public string AuthorId { get; set; }
        public User Author { get; set; }

        [ForeignKey("Character")]
        public int? CharacterId { get; set; } = 0;
        public Character? Character { get; set; }

        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}