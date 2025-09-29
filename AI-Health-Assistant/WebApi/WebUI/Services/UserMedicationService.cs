using System.Text.Json;
using WebUI.Models;

namespace WebUI.Services
{
    public interface IUserMedicationService
    {
        Task<List<UserMedicationModel>> GetUserMedicationAsync(int userId);
        Task<List<MedicationModel>> GetAllMedicationsAsync();
        Task<bool> AddUserMedicationAsync(int userId, int medicationId);
        Task<bool> FinishUserMedicationAsync(int userMedicationId);
    }
    public class UserMedicationService : IUserMedicationService
    {
        private readonly HttpClient _httpClient;

        public UserMedicationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7222/api/");
        }

        public async Task<List<UserMedicationModel>> GetUserMedicationAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"UserMedications/{userId}");
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<UserMedicationModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }

        public async Task<List<MedicationModel>> GetAllMedicationsAsync()
        {
            var response = await _httpClient.GetAsync("Medication");
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<MedicationModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }

        public async Task<bool> AddUserMedicationAsync(int userId, int medicationId)
        {
            var userMedication = new
            {
                UserID = userId,
                MedicationID = medicationId,
                ActiveDate = DateTime.Now,
                InactiveDate = (DateTime?)null
            };

            var response = await _httpClient.PostAsJsonAsync("UserMedications", userMedication);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinishUserMedicationAsync(int userMedicationId)
        {          
            var response = await _httpClient.GetAsync($"UserMedications");
            if (!response.IsSuccessStatusCode) return false;

            var responseContent = await response.Content.ReadAsStringAsync();
            var medications = JsonSerializer.Deserialize<List<ResultUserMedicationDto>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var medication = medications?.FirstOrDefault(m => m.UserMedicationID == userMedicationId);
            if (medication == null) return false;
            var updateMedication = new
            {
                UserMedicationID = medication.UserMedicationID,
                UserID = medication.UserID,
                MedicationID = medication.MedicationID,
                ActiveDate = medication.ActiveDate,
                InactiveDate = DateTime.Now
            };

            var updateResponse = await _httpClient.PutAsJsonAsync("UserMedications", updateMedication);
            return updateResponse.IsSuccessStatusCode;
        }
    }
}
