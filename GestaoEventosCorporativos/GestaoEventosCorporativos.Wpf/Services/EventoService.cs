using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class EventoService
    {
        private readonly HttpClient _httpClient;

        public EventoService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSession.Token);
        }

        public async Task<ApiResponse<EventoResponse>> CadastrarEventoAsync(EventoRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("eventos", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<EventoResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<PagedResult<EventoResponse>>> ListarEventosAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync($"eventos?pageNumber={pageNumber}&pageSize={pageSize}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<PagedResult<EventoResponse>>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<bool>> DeletarEventoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"eventos/{id}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<EventoResponse>> ObterEventoPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"eventos/{id}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<EventoResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<ApiResponse<EventoResponse>> AtualizarEventoAsync(int id, EventoRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"eventos/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<EventoResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<ParticipanteResponse>> AdicionarParticipanteAsync(int eventoId, string cpf)
        {
            var json = JsonSerializer.Serialize(new { cpf });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"eventos/{eventoId}/participantes", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<ParticipanteResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<FornecedorResponse>> AdicionarFornecedorAsync(int eventoId, string cnpj)
        {
            var json = JsonSerializer.Serialize(new { cnpj });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"eventos/{eventoId}/fornecedores", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<FornecedorResponse>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<bool>> RemoverParticipanteAsync(int eventoId, string cpf)
        {
            var response = await _httpClient.DeleteAsync($"eventos/{eventoId}/participantes/{cpf}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<bool>> RemoverFornecedorAsync(int eventoId, string cnpj)
        {
            var response = await _httpClient.DeleteAsync($"eventos/{eventoId}/fornecedores/{cnpj}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

    }
}
