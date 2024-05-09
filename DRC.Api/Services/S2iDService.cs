using DRC.Api.Interfaces;
using DRC.Api.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DRC.Api.Services
{
    public class S2iDService : IS2iDService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<S2iDService> _logger;

        public S2iDService(HttpClient httpClient, ILogger<S2iDService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        private async Task<string> GetTokenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var tokenMatch = Regex.Match(responseBody, @"var token = ""([^""]+)"";");
                if (tokenMatch.Success)
                {
                    return tokenMatch.Groups[1].Value;
                }
                throw new Exception("Token not found in the response.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Exception Caught: {e.Message}");
                return null;
            }
        }

        private async Task<T> MakeRequestWithTokenAsync<T>(string resourcePath)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Unable to retrieve token.");
            }

            _httpClient.DefaultRequestHeaders.Add("Auth-Token", token);

            var response = await _httpClient.GetAsync(resourcePath);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent);
            }
            catch (JsonException je)
            {
                _logger.LogError($"JSON Parsing Error: {je.Message}");
                throw;
            }
        }

        public async Task<List<Cobrade>> GetCobradesAsync()
        {
            return await MakeRequestWithTokenAsync<List<Cobrade>>("/rest/cobrades");
        }

        public async Task<Root> GetRecognitions()
        {
            return await MakeRequestWithTokenAsync<Root>("/rest/portal/reconhecimentos");
        }

        public async Task<Root> GetResilients()
        {
            return await MakeRequestWithTokenAsync<Root>("/rest/portal/resilientes");
        }
    }
}
