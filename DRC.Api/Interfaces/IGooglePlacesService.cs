namespace DRC.Api.Interfaces
{
    public interface IGooglePlacesService
    {
        Task<string> GetHospitalsAsync(double latitude, double longitude);
    }
}
