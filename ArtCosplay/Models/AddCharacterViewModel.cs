using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class AddCharacterViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [MaxLength(2000)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [MaxLength(25)]
        public string Type { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
