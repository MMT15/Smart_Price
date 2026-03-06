using System.Text;
using System.Text.Json;

namespace SmartPrice.Services
{
    public class NotificationService
    {
        private readonly HttpClient _client;

        public NotificationService()
        {
            _client = new HttpClient();
        }

        public async Task SendNotificationAsync(string webhookUrl, string message)
        {
            var payload = new { content = message };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(webhookUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send Discord notification: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }
    }
}
