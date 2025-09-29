using Dapper;
using System.Security.Cryptography;
using System.Text;
using WebApi.Dtos.UserDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public async void CreateUser(CreateUserDto createUserDto)
        {
            string query = "INSERT INTO [User] (UserName, Password, Name, Email, Telefon, Cinsiyet) VALUES (@userName, @password, @name, @email, @telefon, @cinsiyet)";
            var parameters = new DynamicParameters();
            parameters.Add("@userName", createUserDto.UserName);
            parameters.Add("@password", createUserDto.Password);
            parameters.Add("@name", createUserDto.Name);
            parameters.Add("@email", createUserDto.Email);
            parameters.Add("@telefon", createUserDto.Telefon);
            parameters.Add("@cinsiyet", createUserDto.Cinsiyet);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteUser(int id)
        {
            string query = "DELETE FROM [User] WHERE UserID = @userID";
            var parameters = new DynamicParameters();
            parameters.Add("@userID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultUserDto>> GetAllUserAsync()
        {
            string query = "SELECT UserID, UserName, Name, Email, Telefon, Cinsiyet FROM [User]";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<ResultUserDto>(query);
                return users.ToList();
            }
        }

        public async Task<ResultUserDto> GetUser(int id)
        {
            string query = "SELECT UserID, UserName, Name, Email, Telefon, Cinsiyet FROM [User] WHERE UserID = @userID";
            var parameters = new DynamicParameters();
            parameters.Add("@userID", id);
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ResultUserDto>(query, parameters);
            }
        }

        public async Task<ResultUserDto> GetUser(string username)
        {
            string query = "SELECT UserID, UserName, Name, Email, Telefon, Cinsiyet FROM [User] WHERE UserName = @userName";
            var parameters = new DynamicParameters();
            parameters.Add("@userName", username);
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ResultUserDto>(query, parameters);
            }
        }

        public async Task<ResultUserDto> GetByUsernameAndPassword(string username, string password)
        {
            string query = "SELECT UserID, UserName, Name, Email, Telefon, Cinsiyet FROM [User] WHERE UserName = @userName AND Password = @password";
            var parameters = new DynamicParameters();
            parameters.Add("@userName", username);
            parameters.Add("@password", password);
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ResultUserDto>(query, parameters);
            }
        }

        public async void UpdateUser(UpdateUserDto updateUserDto)
        {
            string query = "UPDATE [User] SET UserName = @userName, Password = @password, Name = @name, Email = @email, Telefon = @telefon, Cinsiyet = @cinsiyet WHERE UserID = @userID";
            var parameters = new DynamicParameters();
            parameters.Add("@userID", updateUserDto.UserID);
            parameters.Add("@userName", updateUserDto.UserName);
            parameters.Add("@password", updateUserDto.Password);
            parameters.Add("@name", updateUserDto.Name);
            parameters.Add("@email", updateUserDto.Email);
            parameters.Add("@telefon", updateUserDto.Telefon);
            parameters.Add("@cinsiyet", updateUserDto.Cinsiyet);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}