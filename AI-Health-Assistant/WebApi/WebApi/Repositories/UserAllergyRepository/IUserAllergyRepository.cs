using WebApi.Dtos.UserAllergyDto;

namespace WebApi.Repositories.UserAllergyRepository
{
    public interface IUserAllergyRepository
    {
        Task<List<ResultUserAllergyDto>> GetAllUserAllergyAsync();
        Task<List<ResultUserAllergyWithNameDto>> GetByNameAsync();
        Task<List<ResultUserAllergyWithNameGetByUserID>> GetByUserIdAsync(int userId);
        Task<bool> CreateUserAllergy(CreateUserAllergyDto createUserAllergyDto);
        void DeleteUserAllergy(int id);
        void UpdateUserAllergy(UpdateUserAllergyDto updateUserAllergyDto);
        Task<bool> CheckUserAllergyAsync(int userId, int allergyId);
        Task<ResultUserAllergyDetailDto?> GetUserAllergyDetailAsync(int userId, int allergyId);

    }
}
