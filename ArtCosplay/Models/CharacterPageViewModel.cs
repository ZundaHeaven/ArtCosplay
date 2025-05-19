namespace ArtCosplay.Models
{
    public class CharacterPageViewModel
    {
        public int Page { get; set; } = 1;
        public string? TextFilter { get; set; }
        public string? PostTypeFilter { get; set; }
        public string? SortType { get; set; }
    }
}
