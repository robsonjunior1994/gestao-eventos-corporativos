using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Wpf.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };
        }

        public async Task<ApiResponse<UserResponse>> RegisterUserAsync(UserRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<UserResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
