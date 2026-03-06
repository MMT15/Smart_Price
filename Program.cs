using System.Text.Json;
using System.Globalization;
using SmartPrice.Models;
using SmartPrice.Services;

// Setăm cultura globală pe Română pentru a afișa prețurile în Lei corect
CultureInfo.CurrentCulture = new CultureInfo("ro-RO");
CultureInfo.CurrentUICulture = new CultureInfo("ro-RO");

Console.WriteLine("--- Smart Price & Availability Tracker Started ---");

// Load Configuration
string configPath = "config.json";
if (!File.Exists(configPath))
{
    Console.WriteLine($"Config file not found at {configPath}. Exiting...");
    return;
}

var configJson = await File.ReadAllTextAsync(configPath);
var config = JsonSerializer.Deserialize<AppConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

if (config == null || config.Products.Count == 0)
{
    Console.WriteLine("Invalid configuration or no products to track.");
    return;
}

var scraper = new ScraperService();
var notifier = new NotificationService();

while (true)
{
    Console.WriteLine($"Checking {config.Products.Count} products at {DateTime.Now:HH:mm:ss}...");

    foreach (var product in config.Products)
    {
        var result = await scraper.CheckProductAsync(product);

        if (result.price.HasValue)
        {
            Console.WriteLine($"Product: {product.Name} | Current Price: {result.price:C2} | Target: {product.TargetPrice:C2}");

            if (result.price <= product.TargetPrice)
            {
                string msg = $"🚨 **PRICE DROP ALERT!** 🚨\n{product.Name} is now **{result.price:C2}** (Target: {product.TargetPrice:C2})\n🔗 {product.Url}";
                Console.WriteLine(msg);
                if (!string.IsNullOrEmpty(config.DiscordWebhookUrl))
                {
                    await notifier.SendNotificationAsync(config.DiscordWebhookUrl, msg);
                }
            }

            product.LastPrice = result.price;
        }
        else
        {
            Console.WriteLine($"Could not fetch price for {product.Name}. Check if product is available.");
        }
    }

    Console.WriteLine($"Waiting {config.CheckIntervalMinutes} minutes for next check...");
    await Task.Delay(TimeSpan.FromMinutes(config.CheckIntervalMinutes));
}
