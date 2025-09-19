using GestaoEventosCorporativos.Wpf.DTOs.Relatorios;
using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Reponse.Relatorios;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class RelatorioService
    {
        private readonly HttpClient _httpClient;

        public RelatorioService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSession.Token);
        }

        public async Task<ApiResponse<List<EventoSaldoOrcamentoResponse>>> ListarSaldoOrcamentoEventosAsync(int pageNumber = 1, int pageSize = 1000)
        {
            var url = $"Relatorios/eventos-saldo-orcamento?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EventoSaldoOrcamentoResponse>>>(url);
            return response;
        }

        public async Task<ApiResponse<List<TipoParticipanteFrequenteResponse>>> ListarTiposParticipantesFrequentesAsync()
        {
            var response = await _httpClient.GetAsync("relatorios/tipos-participantes-frequentes?pageNumber=1&pageSize=1000");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<TipoParticipanteFrequenteResponse>>
                {
                    IsSuccess = false,
                    Message = $"Erro: {response.StatusCode}",
                    StatusCode = response.StatusCode.ToString()
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<TipoParticipanteFrequenteResponse>>>();
            return result;
        }

        public async Task<ApiResponse<List<FornecedorMaisUtilizadoResponse>>> ListarFornecedoresMaisUtilizadosAsync()
        {
            var response = await _httpClient.GetAsync("Relatorios/fornecedores-mais-utilizados?pageNumber=1&pageSize=1000");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<List<FornecedorMaisUtilizadoResponse>>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<AgendaParticipanteResponse>> ObterAgendaParticipanteAsync(string cpf)
        {
            var response = await _httpClient.GetAsync($"Relatorios/agenda-participante/{cpf}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<AgendaParticipanteResponse>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }


}

