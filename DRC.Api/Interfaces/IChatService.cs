namespace DRC.Api.Interfaces
{
    public interface IChatService
    {
        Task StartChat(Guid? guid = null);
        Task<(string? guid, string message)> SendMessage(Guid? guid, string message);
    }
}
