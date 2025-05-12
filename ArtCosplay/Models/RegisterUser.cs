using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class RegisterUser
    {
        [Required]
        [RegularExpression("^[A-Za-z0-9]{4,16}$", ErrorMessage = "Username must be 4-16 characters long and contain only letters (A-Z, a-z) and numbers (0-9)")]
        [Length(4, 16)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,20}$", ErrorMessage = "The password must be 8 to 20 characters long and can only contain Latin letters (A-Z, a-z) and numbers (0-9)")]
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string About { get; set; }
    }
}
