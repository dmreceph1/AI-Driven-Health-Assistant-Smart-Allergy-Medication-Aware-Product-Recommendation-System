using System.Text.Json;
using WebUI.Models;

namespace WebUI.Services
{
    public interface IUserAllergyService
    {
        Task<List<UserAllergyModel>> GetUserAllergyAsync(int userId);
        Task<List<AllergyModel>> GetAllAllergiesAsync();
        Task<bool> AddUserAllergyAsync(int userId, int allergyId);
        Task<bool> FinishUserAllergyAsync(int userAllergyId);
    }
    public class UserAllergyService : IUserAllergyService
    {
        private readonly HttpClient _httpClient;

        public UserAllergyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7222/api/");
        }

        public async Task<bool> AddUserAllergyAsync(int userId, int allergyId)
        {
            var userAllergy = new
            {
                UserID = userId,
                AllergyID = allergyId,
                DiagnosisDate = DateTime.Now,
                UpdateDate = (DateTime?)null
            };

            var response = await _httpClient.PostAsJsonAsync("UserAllergy", userAllergy);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinishUserAllergyAsync(int userAllergyId)
        {
            var response = await _httpClient.GetAsync($"UserAllergy");
            if (!response.IsSuccessStatusCode) return false;

            var responseContent = await response.Content.ReadAsStringAsync();
            var allergies = JsonSerializer.Deserialize<List<ResultUserAllergyDto>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var allergy = allergies?.FirstOrDefault(m => m.UserAllergyID == userAllergyId);
            if (allergy == null) return false;
            var updateAllergy = new
            {
                UserAllergyID = allergy.UserAllergyID,
                UserID = allergy.UserID,
                AllergyID = allergy.AllergyID,
                DiagnosisDate = allergy.DiagnosisDate,
                UpdateDate = DateTime.Now
            };

            var updateResponse = await _httpClient.PutAsJsonAsync("UserAllergy", updateAllergy);
            return updateResponse.IsSuccessStatusCode;
        }

        public async Task<List<AllergyModel>> GetAllAllergiesAsync()
        {
            var response = await _httpClient.GetAsync("Allergy");
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<AllergyModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }

        public async Task<List<UserAllergyModel>> GetUserAllergyAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"UserAllergy/{userId}");
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<UserAllergyModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }
    }
}
