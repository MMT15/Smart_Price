using HtmlAgilityPack;
using SmartPrice.Models;
using System.Globalization;

namespace SmartPrice.Services
{
    public class ScraperService
    {
        private readonly HttpClient _client;

        public ScraperService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        public async Task<(decimal? price, bool available)> CheckProductAsync(Product product)
        {
            try
            {
                var response = await _client.GetAsync(product.Url);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var priceNode = doc.DocumentNode.SelectSingleNode(product.PriceSelector);
                if (priceNode == null) return (null, false);

                var priceText = priceNode.InnerText.Trim();
                // Clean the price text (remove non-digits, keep dot/comma for decimal)
                var cleanPrice = new string(priceText.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
                
                if (decimal.TryParse(cleanPrice.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    return (price, true);
                }

                return (null, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping {product.Name}: {ex.Message}");
                return (null, false);
            }
        }
    }
}
