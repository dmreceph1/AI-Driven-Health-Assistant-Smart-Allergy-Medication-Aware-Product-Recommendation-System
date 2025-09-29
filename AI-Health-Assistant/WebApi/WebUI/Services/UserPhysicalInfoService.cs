using System.Text.Json;
using WebUI.Models;

namespace WebUI.Services
{
    public interface IUserPhysicalInfoService
    {
        Task<UserPhysicalInfoDto> GetUserPhysicalInfoAsync(int userId);
        Task<bool> UpdateUserPhysicalInfoAsync(UserPhysicalInfoDto userPhysicalInfo);
    }

    public class UserPhysicalInfoService : IUserPhysicalInfoService
    {
        private readonly HttpClient _httpClient;

        public UserPhysicalInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7222/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<UserPhysicalInfoDto> GetUserPhysicalInfoAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"UserPhysicalInfo/user/{userId}");
                if (!response.IsSuccessStatusCode || response.Content.Headers.ContentLength == 0)
                {
                    return new UserPhysicalInfoDto
                    {
                        UserID = userId,
                        Height = 0,
                        Weight = 0
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return new UserPhysicalInfoDto
                    {
                        UserID = userId,
                        Height = 0,
                        Weight = 0
                    };
                }

                var result = JsonSerializer.Deserialize<UserPhysicalInfoDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result ?? new UserPhysicalInfoDto
                {
                    UserID = userId,
                    Height = 0,
                    Weight = 0
                };
            }
            catch (Exception)
            {
                return new UserPhysicalInfoDto
                {
                    UserID = userId,
                    Height = 0,
                    Weight = 0
                };
            }
        }

        public async Task<bool> UpdateUserPhysicalInfoAsync(UserPhysicalInfoDto userPhysicalInfo)
        {
            try
            {
                // Önce kullanıcının mevcut fiziksel bilgilerini al
                var currentInfo = await GetUserPhysicalInfoAsync(userPhysicalInfo.UserID);

                // Eğer mevcut bilgiler varsa (InfoID > 0), PUT isteği yap
                if (currentInfo.InfoID > 0)
                {
                    userPhysicalInfo.InfoID = currentInfo.InfoID;
                    var response = await _httpClient.PutAsJsonAsync("UserPhysicalInfo", userPhysicalInfo);
                    return response.IsSuccessStatusCode;
                }
                else
                {
                    // Mevcut bilgiler yoksa, POST isteği yap
                    var response = await _httpClient.PostAsJsonAsync("UserPhysicalInfo", userPhysicalInfo);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}