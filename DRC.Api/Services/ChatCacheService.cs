using ChatAIze.GenerativeCS.Models;
using DRC.Api.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DRC.Api.Services
{
    public class ChatCacheService : IChatCacheService
    {
        private readonly IDistributedCache _cache;
        public ChatCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task SaveConversationAsync(ChatConversation conversation)
        {
            var serialized = JsonConvert.SerializeObject(conversation);
            await _cache.SetStringAsync(conversation.UserTrackingId, serialized);
        }

        public async Task<ChatConversation> GetConversationAsync(Guid id)
        {
            var serialized = await _cache.GetStringAsync(id.ToString());
            return serialized != null ? JsonConvert.DeserializeObject<ChatConversation>(serialized) : null;
        }
    }
}
