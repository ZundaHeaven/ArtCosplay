using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class AddRoleViewModel
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string roleName { get; set; }   
    }
}
