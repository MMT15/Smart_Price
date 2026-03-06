namespace SmartPrice.Models
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public decimal TargetPrice { get; set; }
        public decimal? LastPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string PriceSelector { get; set; } = string.Empty; // CSS selector or XPath
    }
}
