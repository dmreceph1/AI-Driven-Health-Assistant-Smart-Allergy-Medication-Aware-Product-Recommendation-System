using System.Text.Json;
using System.Text;
using WebUI.Models;

namespace WebUI.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7222/api/Auth/");
        }
        public async Task<AuthResponse> LoginAsync(LoginViewModel loginModel)
        {
            var json = JsonSerializer.Serialize(loginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }
        public async Task<(bool success, string errorMessage)> RegisterAsync(RegisterViewModel registerModel)
        {
            var content = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("register", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = "Kayıt işlemi başarısız oldu. ";
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString);
                    if (errorResponse != null && errorResponse.ContainsKey("message"))
                    {
                        errorMessage = errorResponse["message"].ToString();
                    }
                }
                catch
                {
                    errorMessage += responseString;
                }
                return (false, errorMessage);
            }

            return (true, null);
        }
    }
}
