using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class TipoEventoService
    {
        private readonly HttpClient _httpClient;

        public TipoEventoService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSession.Token);
        }

        public async Task<ApiResponse<TipoEventoResponse>> CadastrarTipoEventoAsync(TipoEventoRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("tipoEventos", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TipoEventoResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<PagedResult<TipoEventoListResponse>>> ListarTipoEventosAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"tipoEventos?pageNumber={pageNumber}&pageSize={pageSize}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<PagedResult<TipoEventoListResponse>>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        public async Task<ApiResponse<bool>> DeletarTipoEventoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"tipoEventos/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<TipoEventoResponse>> EditarTipoEventoAsync(int id, TipoEventoRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"tipoEventos/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TipoEventoResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
