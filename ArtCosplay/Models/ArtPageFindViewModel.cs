namespace ArtCosplay.Models
{
    public class ArtPageFindViewModel
    {
        public int? Page { get; set; }
        public string? Filter { get; set; }
        public string? Sort { get; set; }
        public string? FilterType { get; set; }
        public int? CharacterId { get; set; }
    }
}
