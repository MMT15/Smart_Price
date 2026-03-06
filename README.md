# 🏷️ Smart Price & Availability Tracker

**Smart Price & Availability Tracker** is a C# .NET 8.0 console application that monitors product prices from e-commerce websites and notifies you via **Discord Webhooks** when the price drops below your target.

## 🚀 Features
- **Automated Web Scraping**: Periodically checks prices using `HtmlAgilityPack`.
- **Async & Non-blocking**: Uses `HttpClient` and `async/await` for high efficiency.
- **Discord Integration**: Get real-time alerts directly on your Discord server.
- **Configurable**: Simple JSON-based configuration for easy management.

## 🛠️ Tech Stack
- **Language**: C# 12 (.NET 8.0)
- **Parser**: [HtmlAgilityPack](https://html-agility-pack.net/)
- **Data Handling**: System.Text.Json
- **Notifications**: Discord Webhooks (HTTP POST)

## 📋 Configuration (`config.json`)
Update the `config.json` file with your target products and Discord webhook:

```json
{
  "CheckIntervalMinutes": 30,
  "DiscordWebhookUrl": "https://discord.com/api/webhooks/...",
  "Products": [
    {
      "Name": "iPhone 15 Pro",
      "Url": "https://example.com/product-url",
      "TargetPrice": 5000.0,
      "PriceSelector": "//span[@class='price-value']"
    }
  ]
}
```

## 🏃 How to Run
1. Clone the repository.
2. Ensure you have the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.
3. Update `config.json` with your products and webhook URL.
4. Run the application:
   ```bash
   dotnet run
   ```

## 🔮 Future Enhancements
- [ ] Multiple notification channels (Email, Slack, Telegram).
- [ ] Support for JavaScript-heavy websites (Playwright/Selenium integration).
- [ ] Multi-currency support.
- [ ] Database integration for historical price tracking.

---
Created for educational purposes and personal use. Happy tracking! 🛍️
