using DRC.Api.Interfaces;
using Newtonsoft.Json;

namespace DRC.Api.Services
{
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeocodingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesByPostalCodeAsync(string postalCode)
        {
            var url = $"?address={postalCode}&key={_configuration["GoogleKey"]}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            // Parse the JSON response to extract latitude and longitude
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            if (data.status == "OK")
            {
                double latitude = data.results[0].geometry.location.lat;
                double longitude = data.results[0].geometry.location.lng;
                return (Latitude: latitude, Longitude: longitude);
            }
            throw new Exception("Could not find coordinates.");
        }
    }

}
