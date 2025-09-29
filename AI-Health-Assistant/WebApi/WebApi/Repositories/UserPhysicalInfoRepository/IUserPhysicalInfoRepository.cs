using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.ProductContentsDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Dtos.UserPhysicalInfo;

namespace WebApi.Repositories.UserPhysicalInfoRepository
{
    public interface IUserPhysicalInfoRepository
    {
        Task<List<ResultUserPhysicalInfoDto>> GetAllUserPhysicalInfoAsync();
        Task<List<ResultUserPhysicalInfoWithUserDto>> GetResultUserPhysicalInfoWithUserDtos();
        void CreateUserPhysicalInfo(CreateUserPhysicalInfoDto createUserPhysicalInfo);
        void DeleteUserPhysicalInfo(int id);
        void UpdateUserPhysicalInfo(UpdateUserPhysicalInfoDto updateUserPhysicalInfoDto);
        Task<UserPhysicalInfoGetByIDDto> GetInfo(int id);
		Task<UserPhysicalInfoGetByUserIdDto> GetByUserIdAsync(int userId);
	}
}
