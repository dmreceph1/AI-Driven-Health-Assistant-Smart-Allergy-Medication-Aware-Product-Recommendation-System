using WebApi.Dtos.UserAllergyDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Dtos.UserPhysicalInfo;

namespace WebApi.Repositories.UserMedicationsRepository
{
    public interface IUserMedicationsRepository
    {
        Task<List<ResultUserMedicationDto>> GetAllUserMedicationsAsync();
        Task<List<ResultUserMedicationWithNameDto>> GetByNameAsync();
        void CreateUserMedication(CreateUsermedicationDto createUsermedicationDto);
        void DeleteUserMedication(int id);
        void UpdateUserMedication(UpdateUserMedicationDto updateUserMedicationDto);
        Task<List<ResultUserMedicationWithNameGetByUserID>> GetByUserIdAsync(int userId);
        Task<bool> CheckUserMedicationAsync(int userId, int medicationId);
        Task<ResultUserMedicationDetailDto?> GetUserMedicationDetailAsync(int userId, int medicationId);
    }
}
