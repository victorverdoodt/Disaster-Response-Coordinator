using ChatAIze.GenerativeCS.Models;

namespace DRC.Api.Interfaces
{
    public interface IChatCacheService
    {
        Task SaveConversationAsync(ChatConversation conversation);
        Task<ChatConversation> GetConversationAsync(Guid id);
    }
}
