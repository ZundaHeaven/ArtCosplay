namespace ArtCosplay.Models
{
    public class ShopPageFindViewModel
    {
        public int? Page { get; set; }
        public string? FilterText { get; set; }
        public string? FilterType { get; set; }
        public string? FilterCity { get; set; }
        public int? FilterCost { get; set; }
    }
}
