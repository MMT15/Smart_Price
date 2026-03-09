# Smart Price and Availability Tracker

Smart Price and Availability Tracker is a C# .NET 8.0 console application designed to monitor product prices from various e-commerce platforms. It automatically tracks price changes and sends real-time notifications via Discord Webhooks when a product's price falls to or below a specified target.

## Features

- Automated Web Scraping: Periodically monitors prices using a multi-strategy scraping approach.
- Smart Price Discovery: Utilizes metadata analysis, JSON-LD parsing, and regex-based text matching to extract prices accurately.
- Discord Integration: Sends instant alerts to a Discord channel when price drops are detected.
- Asynchronous Operations: Built with HttpClient and async/await for high performance and non-blocking execution.
- Romanian Market Optimization: Specifically configured to handle price formats and structures common on major Romanian e-commerce sites like eMAG and Altex.

## Tech Stack

- Language: C# 12
- Framework: .NET 8.0
- Data Handling: System.Text.Json
- Notifications: Discord Webhooks (HTTP POST)
- Web Interaction: HttpClient with custom headers to simulate real browser behavior.

## Configuration (config.json)

The application requires a config.json file in the root directory. You can use the following structure:

```json
{
  "CheckIntervalMinutes": 30,
  "DiscordWebhookUrl": "https://discord.com/api/webhooks/your-webhook-url",
  "Products": [
    {
      "Name": "iPhone 15 Pro",
      "Url": "https://www.example.ro/product-url",
      "TargetPrice": 5000.0
    }
  ]
}
```

### Configuration Fields

- CheckIntervalMinutes: The frequency (in minutes) at which the application checks for price updates.
- DiscordWebhookUrl: Your unique Discord Webhook URL for receiving notifications.
- Products: An array of objects containing:
  - Name: The display name of the product.
  - Url: The direct link to the product page.
  - TargetPrice: The price threshold that triggers a notification.
  - PriceSelector: (Optional) Reserved for future use with CSS/XPath selectors. The current version uses automated discovery strategies.

## How to Run

1. Clone the repository to your local machine.
2. Ensure you have the .NET 8.0 SDK installed.
3. Create a config.json file based on the provided example.
4. Open a terminal in the project directory.
5. Build and run the application:
   ```bash
   dotnet run
   ```

## Technical Details

The application uses several strategies to ensure high reliability in price extraction:
- Metadata Strategy: Checks for OpenGraph and product schema meta tags.
- JSON-LD Strategy: Parses structured data embedded in the HTML, which is commonly used by modern e-commerce platforms.
- Regex Strategy: Fallback mechanism that searches for currency symbols and numeric patterns within the HTML body.

## Future Enhancements

- Support for additional notification channels such as Email, Slack, and Telegram.
- Integration with Playwright or Selenium for JavaScript-heavy websites that require full browser rendering.
- Multi-currency support for international price tracking.
- Database integration to store historical price data and generate trends.

---

