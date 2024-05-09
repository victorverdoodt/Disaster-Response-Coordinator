using System.Text;
using System.Text.Json;

namespace DRC.App.Services
{
    public class ChatClientService
    {
        private readonly HttpClient _httpClient;
        public ChatClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(Guid Guid, string Response)> Conversation(string message, Guid? guid = null)
        {
            var requestUri = $"api/Chat/Conversation" + (guid.HasValue ? $"?guid={guid}" : string.Empty);
            var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(requestUri, content))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    var root = doc.RootElement;
                    var guidValue = root.GetProperty("guid").GetGuid();
                    var responseMessage = root.GetProperty("response").GetString();
                    return (Guid: guidValue, Response: responseMessage);
                }
            }
        }

    }
}
