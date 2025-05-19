using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class CreateShoppingItemViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        public string Price { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        [MaxLength(25)]
        public string Type { get; set; }

        [Required]
        [MaxLength(2000)]
        [Display(Name = "Текст обсуждения")]
        public string Content { get; set; }
        
        [Required]
        public string City { get; set; }
    }
}
