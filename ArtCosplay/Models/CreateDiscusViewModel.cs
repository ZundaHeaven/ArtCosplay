using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class CreateDiscusViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        [Display(Name = "Текст обсуждения")]
        public string Content { get; set; }
    }
}
