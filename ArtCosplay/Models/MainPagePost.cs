using ArtCosplay.Data.DB;
using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class MainPagePost
    {
        public Post? Post { get; set; }
        public Discussion? Discussion { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }  
    }
}
