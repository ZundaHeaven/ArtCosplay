using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtCosplay.Data.DB
{
    public class PostCharacter
    {
        [Key]
        public int PostCharacterId { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey("AnimeCharacter")]
        public int CharacterId { get; set; }
        public AnimeCharacter Character { get; set; }
    }
}