using Dapper;
using WebApi.Dtos.UserAllergyDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.UserMedicationsRepository
{
    public class UserMedicationsRepository : IUserMedicationsRepository
    {
        private readonly Context _context;
        public UserMedicationsRepository(Context context)
        {
            _context = context;
        }

        public async void CreateUserMedication(CreateUsermedicationDto createUsermedicationDto)
        {
            string query = "insert into UserMedication (UserID, MedicationID, ActiveDate, InactiveDate) Values (@userID, @medicationID, @activeDate, @ınactiveDate)";
            var parameters = new DynamicParameters();
            parameters.Add("@userID", createUsermedicationDto.UserID);
            parameters.Add("@medicationID", createUsermedicationDto.MedicationID);
            parameters.Add("@activeDate", createUsermedicationDto.ActiveDate);
            parameters.Add("@ınactiveDate", createUsermedicationDto.InactiveDate);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteUserMedication(int id)
        {
            string query = "Delete From UserMedication Where UserMedicationID=@userMedicationID";
            var parameters = new DynamicParameters();
            parameters.Add("@userMedicationID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultUserMedicationDto>> GetAllUserMedicationsAsync()
        {
            string query = "Select*From UserMedication";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultUserMedicationDto>(query);
                return x.ToList();
            }
        }

        public async Task<List<ResultUserMedicationWithNameDto>> GetByNameAsync()
        {
            var connection = _context.CreateConnection();
            var query = @"SELECT ua.UserMedicationID, ua.UserID, ua.MedicationID, ua.ActiveDate, ua.InactiveDate,
                         a.MedicationName, u.UserName
                         FROM UserMedication ua
                         INNER JOIN Medications a ON ua.MedicationID = a.MedicationID
                         INNER JOIN [User] u ON ua.UserID = u.UserID";
            var values = await connection.QueryAsync<ResultUserMedicationWithNameDto>(query);
            return values.ToList();
        }

        public async Task<List<ResultUserMedicationWithNameGetByUserID>> GetByUserIdAsync(int userId)
        {
            string query = @"
            SELECT 
                um.UserMedicationID,
                um.UserID,
                m.MedicationName,
                um.ActiveDate,
                um.InactiveDate
            FROM UserMedication um
            INNER JOIN Medications m ON um.MedicationID = m.MedicationID
            WHERE um.UserID = @userID;";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userID", userId);

                return (await connection.QueryAsync<ResultUserMedicationWithNameGetByUserID>(query, parameters)).ToList();
            }
        }

        public async void UpdateUserMedication(UpdateUserMedicationDto updateUserMedicationDto)
        {
            string query = "Update UserMedication Set UserID=@userID, MedicationID=@medicationID, ActiveDate=@activeDate, InactiveDate=@ınactiveDate Where UserMedicationID=@userMedicationID";
            var parameters = new DynamicParameters();
            parameters.Add("@userMedicationID", updateUserMedicationDto.UserMedicationID);
            parameters.Add("@userID", updateUserMedicationDto.UserID);
            parameters.Add("@medicationID", updateUserMedicationDto.MedicationID);
            parameters.Add("@activeDate", updateUserMedicationDto.ActiveDate);
            parameters.Add("@ınactiveDate", updateUserMedicationDto.InactiveDate);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<bool> CheckUserMedicationAsync(int userId, int medicationId)
        {
            string query = "SELECT COUNT(*) FROM UserMedication WHERE UserID = @userId AND MedicationID = @medicationId AND InactiveDate IS NULL";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@medicationId", medicationId);

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
            }
        }

        public async Task<ResultUserMedicationDetailDto?> GetUserMedicationDetailAsync(int userId, int medicationId)
        {
            string query = @"SELECT TOP 1 UserMedicationID, UserID, MedicationID, ActiveDate, InactiveDate 
                     FROM UserMedication
                     WHERE UserID = @userId AND MedicationID = @medicationId AND InactiveDate IS NULL";

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@medicationId", medicationId);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResultUserMedicationDetailDto>(query, parameters);
                return result;
            }
        }
    }
}
