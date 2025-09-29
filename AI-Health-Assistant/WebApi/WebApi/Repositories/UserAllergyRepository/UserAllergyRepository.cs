using Dapper;
using WebApi.Dtos.ProductContentsDto;
using WebApi.Dtos.UserAllergyDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.UserAllergyRepository
{
    public class UserAllergyRepository : IUserAllergyRepository
    {
        private readonly Context _context;

        public UserAllergyRepository(Context context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAllergy(CreateUserAllergyDto createUserAllergyDto)
        {
            string query = "insert into UserAllergy (AllergyID, UserID, DiagnosisDate, UpdateDate) Values (@allergyID, @userID, @diagnosisDate, @updateDate)";
            var parameters = new DynamicParameters();
            parameters.Add("@allergyID", createUserAllergyDto.AllergyID);
            parameters.Add("@userID", createUserAllergyDto.UserID);
            parameters.Add("@diagnosisDate", createUserAllergyDto.DiagnosisDate);
            parameters.Add("@updateDate", createUserAllergyDto.UpdateDate);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var affected = await connection.ExecuteAsync(query, parameters);
                    return affected > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public async void DeleteUserAllergy(int id)
        {
            string query = "Delete From UserAllergy Where UserAllergyID=@userAllergyID";
            var parameters = new DynamicParameters();
            parameters.Add("@userAllergyID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultUserAllergyDto>> GetAllUserAllergyAsync()
        {
            string query = "Select*From UserAllergy";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultUserAllergyDto>(query);
                return x.ToList();
            }
        }

        public async Task<List<ResultUserAllergyWithNameDto>> GetByNameAsync()
        {
            var connection = _context.CreateConnection();
            var query = @"SELECT ua.UserAllergyID, ua.AllergyID, ua.UserID, ua.DiagnosisDate, ua.UpdateDate,
                         a.AllergyName, u.UserName
                         FROM UserAllergy ua
                         INNER JOIN Allergy a ON ua.AllergyID = a.AllergyID
                         INNER JOIN [User] u ON ua.UserID = u.UserID";
            var values = await connection.QueryAsync<ResultUserAllergyWithNameDto>(query);
            return values.ToList();
        }

        public async void UpdateUserAllergy(UpdateUserAllergyDto updateUserAllergyDto)
        {        
            string query = "Update UserAllergy Set AllergyID=@allergyID, UserID=@userID, DiagnosisDate=@diagnosisDate, UpdateDate=@updateDate Where UserAllergyID=@userAllergyID";
            var parameters = new DynamicParameters();
            parameters.Add("@userAllergyID", updateUserAllergyDto.UserAllergyID);
            parameters.Add("@allergyID", updateUserAllergyDto.AllergyID);
            parameters.Add("@userID", updateUserAllergyDto.UserID);
            parameters.Add("@diagnosisDate", updateUserAllergyDto.DiagnosisDate);
            parameters.Add("@updateDate", DateTime.Now);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultUserAllergyWithNameGetByUserID>> GetByUserIdAsync(int userId)
        {
            var connection = _context.CreateConnection();
            var query = @"SELECT ua.UserAllergyID, ua.UserID, ua.DiagnosisDate, ua.UpdateDate,
                         a.AllergyName
                         FROM UserAllergy ua
                         INNER JOIN Allergy a ON ua.AllergyID = a.AllergyID
                         WHERE ua.UserID = @userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            var values = await connection.QueryAsync<ResultUserAllergyWithNameGetByUserID>(query, parameters);
            return values.ToList();
        }

        public async Task<bool> CheckUserAllergyAsync(int userId, int allergyId)
        {
            string query = "SELECT COUNT(*) FROM UserAllergy WHERE UserID = @userId AND AllergyID = @allergyId AND UpdateDate IS NULL";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@allergyId", allergyId);

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0; 
            }
        }

        public async Task<ResultUserAllergyDetailDto?> GetUserAllergyDetailAsync(int userId, int allergyId)
        {
            string query = @"SELECT TOP 1 UserAllergyID, UserID, AllergyID, DiagnosisDate, UpdateDate 
                     FROM UserAllergy 
                     WHERE UserID = @userId AND AllergyID = @allergyId AND UpdateDate IS NULL";

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@allergyId", allergyId);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResultUserAllergyDetailDto>(query, parameters);
                return result;
            }
        }


    }
}
