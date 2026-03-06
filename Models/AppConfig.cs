namespace SmartPrice.Models
{
    public class AppConfig
    {
        public List<Product> Products { get; set; } = new();
        public int CheckIntervalMinutes { get; set; } = 30;
        public string DiscordWebhookUrl { get; set; } = string.Empty;
    }
}
