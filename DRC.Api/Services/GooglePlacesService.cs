using DRC.Api.Interfaces;
using System.Text.Json;

namespace DRC.Api.Services
{
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GooglePlacesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetHospitalsAsync(double latitude, double longitude)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude.ToString().Replace(",", ".")},{longitude.ToString().Replace(",", ".")}&radius=10000&type=hospital&opennow=true&key={_configuration["Apps:Google:Key"]}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

            var filteredResults = new List<Dictionary<string, string>>();
            foreach (var result in jsonResponse.GetProperty("results").EnumerateArray())
            {
                var name = result.GetProperty("name").GetString();
                var vicinity = result.GetProperty("vicinity").GetString();
                filteredResults.Add(new Dictionary<string, string>
                {
                    ["name"] = name,
                    ["vicinity"] = vicinity
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(filteredResults, options);
        }
    }
}
