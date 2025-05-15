using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class CommentViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string Content {  get; set; }
    }
}
