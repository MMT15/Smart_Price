using SmartPrice.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;

namespace SmartPrice.Services
{
    public class ScraperService
    {
        private readonly HttpClient _client;

        public ScraperService()
        {
            // Forțăm HTTP/1.1 pentru a evita erorile de protocol de pe Altex/eMAG
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                CookieContainer = new CookieContainer()
            };

            _client = new HttpClient(handler);
            _client.DefaultRequestVersion = HttpVersion.Version11;
            _client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
            _client.Timeout = TimeSpan.FromSeconds(15);

            // Headere care imită perfect un browser real
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ro-RO,ro;q=0.9,en-US;q=0.8,en;q=0.7");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "max-age=0");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua", "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\"");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua-Mobile", "?0");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua-Platform", "\"Windows\"");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://www.google.com/");
            _client.DefaultRequestHeaders.ConnectionClose = false;
        }

        public async Task<(decimal? price, bool available)> CheckProductAsync(Product product)
        {
            try
            {
                Console.WriteLine($"[Scraper] Requesting: {product.Url}");
                
                // Cerem pagina
                var response = await _client.GetAsync(product.Url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Scraper Error] HTTP {response.StatusCode} for {product.Name}");
                    return (null, false);
                }

                var html = await response.Content.ReadAsStringAsync();
                decimal? foundPrice = null;

                // --- STRATEGIA 1: Regex pe Metadate (Standard & Rapid) ---
                var metaMatch = Regex.Match(html, @"<meta[^>]+(?:itemprop|property|name)=[""'](?:price|product:price:amount|og:price:amount)[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (metaMatch.Success)
                {
                    foundPrice = CleanAndParsePrice(metaMatch.Groups[1].Value);
                    if (foundPrice.HasValue && foundPrice > 0)
                    {
                        Console.WriteLine($"[SmartDiscovery] Found in Meta: {foundPrice:C2}");
                        return (foundPrice, true);
                    }
                }

                // --- STRATEGIA 2: Regex pe JSON-LD (Altex/eMAG folosesc asta masiv) ---
                var jsonMatch = Regex.Match(html, @"""price""\s*:\s*""?(\d+[\.,]\d+)""?", RegexOptions.IgnoreCase);
                if (jsonMatch.Success)
                {
                    foundPrice = CleanAndParsePrice(jsonMatch.Groups[1].Value);
                    if (foundPrice.HasValue && foundPrice > 0)
                    {
                        Console.WriteLine($"[SmartDiscovery] Found in JSON Data: {foundPrice:C2}");
                        return (foundPrice, true);
                    }
                }

                // --- STRATEGIA 3: Căutare după simbolul "Lei" (Ultima speranță) ---
                var textMatch = Regex.Match(html, @">([\d\s\.,]+)\s*(?:Lei|RON)<", RegexOptions.IgnoreCase);
                if (textMatch.Success)
                {
                    foundPrice = CleanAndParsePrice(textMatch.Groups[1].Value);
                    if (foundPrice.HasValue && foundPrice > 5)
                    {
                        Console.WriteLine($"[TextDiscovery] Found in HTML: {foundPrice:C2}");
                        return (foundPrice, true);
                    }
                }

                Console.WriteLine($"[Scraper] Could not find price for {product.Name}. Site may be protected.");
                return (null, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Scraper Error] {product.Name}: {ex.Message}");
                return (null, false);
            }
        }

        private decimal? CleanAndParsePrice(string? priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText)) return null;
            
            // Păstrăm doar cifrele, punctul și virgula
            string clean = new string(priceText.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            if (string.IsNullOrEmpty(clean)) return null;

            // Tratăm separarea miilor și zecimalelor
            if (clean.Contains(",") && clean.Contains(".")) {
                clean = clean.Replace(".", ""); // Eliminăm mii
                clean = clean.Replace(",", "."); // Coma devine punct zecimal
            } else if (clean.Contains(",") && clean.Count(c => c == ',') == 1) {
                clean = clean.Replace(",", ".");
            } else if (clean.Contains(".") && clean.Count(c => c == '.') > 1) {
                clean = clean.Replace(".", "");
            }

            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price)) return price;
            return null;
        }
    }
}
