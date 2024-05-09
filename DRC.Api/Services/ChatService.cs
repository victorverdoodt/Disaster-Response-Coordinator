using ChatAIze.GenerativeCS.Clients;
using ChatAIze.GenerativeCS.Models;
using ChatAIze.GenerativeCS.Options.Gemini;
using ChatAIze.GenerativeCS.Enums;
using DRC.Api.Interfaces;
using ViaCep;
using System.Reflection.Emit;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Linq;
using DRC.Api.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace DRC.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly GeminiClient _geminiClient;
        private readonly ICepService _cepService;
        private readonly IChatCacheService _chatCacheService;
        private readonly IS2iDService _s2iDService;
        private readonly IDistributedCache _cache;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IGeocodingService _geocodingService;
        private ChatConversation _chatConversation;
        private ChatCompletionOptions _chatCompletionOptions;

        public ChatService(GeminiClient geminiClient, ICepService cepService, IChatCacheService chatCacheService, IS2iDService s2iDService, IDistributedCache cache, IGooglePlacesService googlePlacesService, IGeocodingService geocodingService)
        {
            _geminiClient = geminiClient;
            _cepService = cepService;
            _chatCacheService = chatCacheService;
            _googlePlacesService = googlePlacesService;
            _chatCompletionOptions = new ChatCompletionOptions();
            _s2iDService = s2iDService;
            _cache = cache;
            _geocodingService = geocodingService;
        }

        [Description("Localiza um endereço via CEP")]
        private async Task<ViaCepResult> GetCurrentAddress(string cep)
        {
            return await _cepService.FindAddressByCep(cep.Trim().Replace("-", string.Empty));
        }

        [Description("Localiza um endereço via CEP")]
        private async Task<string> GetAvailableShelters(string cep)
        {
            var city = await _geocodingService.GetCoordinatesByPostalCodeAsync(cep.Trim().Replace("-", string.Empty));
            return await _googlePlacesService.GetHospitalsAsync(city.Latitude, city.Longitude);
        }


        private async Task<Dictionary<string, Cobrade>> GetCobradesAsync()
        {
            var cacheKey = "cobrades2";
            var cobradesJson = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cobradesJson))
            {
                var cobrades = await _s2iDService.GetCobradesAsync();
                var cobradesDict = cobrades.ToDictionary(c => c.CobradeId.ToString());
                cobradesJson = JsonSerializer.Serialize(cobradesDict);
                await _cache.SetStringAsync(cacheKey, cobradesJson, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }

            return JsonSerializer.Deserialize<Dictionary<string, Cobrade>>(cobradesJson);
        }

        [Description("Verifica os desastres em um lugar pelo seu cep")]
        public async Task<string> GetDesasters(string cep)
        {
            try
            {
                var city = await GetCurrentAddress(cep.Trim().Replace("-", string.Empty));
                var get = await _s2iDService.GetRecognitions();
                var cobrades = await GetCobradesAsync();

                var groupedByCity = get.features
                    .Where(x => x.properties.municipio.Equals(city.City, StringComparison.OrdinalIgnoreCase) &&
                                x.properties.uf.Equals(city.StateInitials, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(x => new { x.properties.municipio, x.properties.uf })
                    .ToDictionary(
                        g => g.Key.municipio + ", " + g.Key.uf,
                        g => g.Select(x => cobrades[x.properties.cobrade.ToString()]).ToList()
                    );

                return JsonSerializer.Serialize(groupedByCity);
            }
            catch
            {
                return "sem nenhum desastre!";
            }
        }

        public async Task StartChat(Guid? guid = null)
        {
            _chatCompletionOptions.AddFunction(GetCurrentAddress);
            _chatCompletionOptions.AddFunction(GetAvailableShelters);
            _chatCompletionOptions.AddFunction(GetDesasters);
            _chatCompletionOptions.IsTimeAware = true;
            _chatCompletionOptions.MaxAttempts = 5;

            if (guid.HasValue)
            {
                _chatConversation = await _chatCacheService.GetConversationAsync(guid.Value);
                return;
            }

            _chatConversation = new()
            {
                UserTrackingId = Guid.NewGuid().ToString()
            };



            string prompt = @$"""Você é um especialista em assistência emergencial e sua tarefa é identificar e responder às necessidades do usuário de forma humanizada e atenciosa durante uma situação de emergência. Inicie a conversa pedindo ao usuário que informe como você pode ajudá-lo, oferecendo opções como abrigosou informações sobre o estado atual do desastre. Baseado na resposta do usuário, você solicitará informações adicionais, como o CEP, para fornecer a assistência mais precisa. Depois, você usará funções específicas para verificar a localidade, o status do desastre e a disponibilidade de abrigos.""

Raciocínio:
Início da Conversa: Cumprimente o usuário e pergunte como você pode ajudar, listando opções específicas de assistência.
Identificação da Necessidade:
Peça ao usuário para escolher entre as opções de ajuda fornecidas (abrigo, assistência médica, informações sobre desastres, etc.).
Obtenção de Informações Adicionais:
Se necessário, peça informações adicionais como o CEP para localizar serviços e assistências próximas.
Funções e Processos:
Localização Exata: Use a função GetCurrentAddress(cep) para obter a localização exata baseada no CEP fornecido.
Verificação do Status do Desastre: Utilize a função GetDesasters(cep) para obter informações atualizadas sobre desastres na região especificada.
Verificação de Abrigos: Empregue a função GetAvailableShelters(cep) para verificar a disponibilidade de abrigos na área.
Resposta ao Usuário:
Forneça uma resposta com as informações obtidas, como a disponibilidade de abrigos e o status do desastre na localidade do usuário.";

            await _chatConversation.FromSystemAsync(prompt, PinLocation.Begin);
            await _chatCacheService.SaveConversationAsync(_chatConversation);
            return;
        }


        public async Task<(string? guid, string message)> SendMessage(Guid? guid, string message)
        {
            await StartChat(guid);
            await _chatConversation.FromUserAsync(message);
            var response = await _geminiClient.CompleteAsync(_chatConversation, _chatCompletionOptions);
            await _chatConversation.FromChatbotAsync(response);
            await _chatCacheService.SaveConversationAsync(_chatConversation);
            return (_chatConversation.UserTrackingId, response);
        }
    }
}
