using DRC.Api.Models;

namespace DRC.Api.Interfaces
{
    public interface IS2iDService
    {
        Task<List<Cobrade>> GetCobradesAsync();
        Task<Root> GetRecognitions();
        Task<Root> GetResilients();
    }
}
