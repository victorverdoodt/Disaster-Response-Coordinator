namespace DRC.Api.Interfaces
{
    public interface IBenfeitoriaService
    {
        Task<string> GetProjectsByKeywordAsync(string keyword);
    }
}
