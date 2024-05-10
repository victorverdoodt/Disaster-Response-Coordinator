using DRC.Api.Interfaces;
using System.Text;
using System.Text.Json;

namespace DRC.Api.Services
{
    public class BenfeitoriaService : IBenfeitoriaService
    {
        private readonly HttpClient _httpClient;

        public BenfeitoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetProjectsByKeywordAsync(string keyword)
        {
            var requestBody = new
            {
                operationName = "GetProjectByFilters",
                variables = new
                {
                    keywords = keyword,
                    first = 9,
                    page = 1
                },
                query = @"query GetProjectByFilters($page: Int!, $first: Int!, $theme: [Int], $region: [Int], $type: [Int], $keywords: String) {
                    search(
                        page: $page
                        first: $first
                        themes: $theme
                        regions: $region
                        project_type: $type
                        query: $keywords
                    ) {
                        data {
                            id
                            title
                            description
                            slug
                            tagline
                            donate_url
                            bypass_project_page
                            city
                            themes {
                                id
                                name
                                __typename
                            }
                            cover {
                                original_url
                                __typename
                            }
                            __typename
                        }
                        paginatorInfo {
                            currentPage
                            hasMorePages
                            total
                            __typename
                        }
                        __typename
                    }
                }"
            };

            string jsonRequest = JsonSerializer.Serialize(requestBody);
            HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("graphql", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
