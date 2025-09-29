using WebApi.Dtos.UserDto;

namespace WebApi.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<List<ResultUserDto>> GetAllUserAsync();
        void CreateUser(CreateUserDto createUserDto);
        void DeleteUser(int id);
        void UpdateUser(UpdateUserDto updateUserDto);
        Task<ResultUserDto> GetUser(int id);
        Task<ResultUserDto> GetUser(string username);
        Task<ResultUserDto> GetByUsernameAndPassword(string username, string password);
    }
}