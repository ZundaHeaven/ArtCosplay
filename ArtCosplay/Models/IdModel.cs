using System.ComponentModel.DataAnnotations;

namespace ArtCosplay.Models
{
    public class IdModel<T>
    {
        [Required]
        public T Id { get; set; }
    }
}
