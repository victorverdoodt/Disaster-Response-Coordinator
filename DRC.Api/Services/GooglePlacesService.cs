using DRC.Api.Interfaces;

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
            var url = $"?location={latitude},{longitude}&radius=10000&type=hospital&key={_configuration["Apps:Google:Key"]}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
