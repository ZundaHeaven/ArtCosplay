using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression("^[A-Za-z0-9]{4,16}$", ErrorMessage = "Имя пользователя должно быть длиной от 4 до 16 символов и содержать только буквы (A-Z, a-z) и цифры (0-9)")]
        [Length(4, 16)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }

        [Required]
        [StringLength(100)]
        public string About { get; set; } = null!;

    }
}
