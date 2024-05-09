namespace DRC.Api.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)> GetCoordinatesByPostalCodeAsync(string postalCode);
    }
}
