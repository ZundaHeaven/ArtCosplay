﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }
        [Required]
        [MaxLength(30)]
        public string City { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public string SellerId { get; set; }
        public User Seller { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<Chat> Chats { get; set; }
    }
}