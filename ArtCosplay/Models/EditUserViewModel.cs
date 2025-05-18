using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class EditUserViewModel
    {
        [Required]
        [StringLength(100)]
        public string Bio { get; set; }
        public IFormFile? Avatar { get; set; }
       
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }
    }
}
