using ViaCep;

namespace DRC.Api.Interfaces
{
    public interface ICepService
    {
        Task<ViaCepResult> FindAddressByCep(string cep);
    }
}
