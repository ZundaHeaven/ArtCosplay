using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class CommentViewModel
    {
        [Required]
        public int Id;
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }
    }
}