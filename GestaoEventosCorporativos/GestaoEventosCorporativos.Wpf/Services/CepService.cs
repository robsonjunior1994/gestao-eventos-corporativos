using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class CepService
    {
        private readonly HttpClient _httpClient;

        public CepService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://viacep.com.br/ws/")
            };
        }

        public async Task<string> BuscarCepAsync(string cep)
        {
            try
            {
                cep = cep.Replace("-", "").Trim();

                var response = await _httpClient.GetAsync($"{cep}/json/");

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<CepResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null || result.Cep == null)
                    return null;

                return $"{result.Logradouro}, {result.Bairro}, {result.Localidade} - {result.Uf}, {result.Cep}";
            }
            catch
            {
                return null;
            }
        }
    }
}
