using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class FornecedorService
    {
        private readonly HttpClient _httpClient;

        public FornecedorService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSession.Token);
        }

        public async Task<ApiResponse<FornecedorResponse>> CadastrarFornecedorAsync(FornecedorRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("fornecedores", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<FornecedorResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<PagedResult<FornecedorListResponse>>> ListarFornecedoresAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"fornecedores?pageNumber={pageNumber}&pageSize={pageSize}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<PagedResult<FornecedorListResponse>>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<FornecedorResponse>> EditarFornecedorAsync(int id, FornecedorRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"fornecedores/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<FornecedorResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<bool>> DeletarFornecedorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"fornecedores/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

    }
}
