using ArtCosplay.Models.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Chat
{
    [Key]
    public int ChatId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [ForeignKey("User")]
    public int BuyerId { get; set; }
    public User Buyer { get; set; }

    [ForeignKey("User")]
    public int SellerId { get; set; }
    public User Seller { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Message> Messages { get; set; }
}