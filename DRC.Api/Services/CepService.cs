using DRC.Api.Interfaces;
using ViaCep;

namespace DRC.Api.Services
{
    public class CepService : ICepService
    {
        protected readonly IViaCepClient _viaCepClient;
        public CepService(IViaCepClient viaCepClient)
        {
            _viaCepClient = viaCepClient;
        }

        public async Task<ViaCepResult> FindAddressByCep(string cep)
        {
            return await _viaCepClient.SearchAsync(cep, CancellationToken.None);
        }
    }
}
