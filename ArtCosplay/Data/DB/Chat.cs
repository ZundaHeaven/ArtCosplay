using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class Chat
    {
        [Key]
        public int ChatId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("User")]
        public string BuyerId { get; set; }
        public User Buyer { get; set; }

        [ForeignKey("User")]
        public string SellerId { get; set; }
        public User Seller { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
    }
}