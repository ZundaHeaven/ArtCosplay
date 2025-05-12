using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

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