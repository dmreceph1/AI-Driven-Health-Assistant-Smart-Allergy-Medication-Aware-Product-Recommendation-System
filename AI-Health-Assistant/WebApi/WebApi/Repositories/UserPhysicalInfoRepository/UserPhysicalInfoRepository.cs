using Dapper;
using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.ProductContentsDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Dtos.UserPhysicalInfo;
using WebApi.Models.PContext;

namespace WebApi.Repositories.UserPhysicalInfoRepository
{
    public class UserPhysicalInfoRepository : IUserPhysicalInfoRepository
    {
        private readonly Context _context;

        public UserPhysicalInfoRepository(Context context)
        {
            _context = context;
        }

        public async void CreateUserPhysicalInfo(CreateUserPhysicalInfoDto createUserPhysicalInfo)
        {
            string query = "insert into UserPhysicalInfo (UserID, Height, Weight) Values (@userID, @height, @weight)";
            var parameters = new DynamicParameters();
            parameters.Add("@userID", createUserPhysicalInfo.UserID);
            parameters.Add("@height", createUserPhysicalInfo.Height);
            parameters.Add("@weight", createUserPhysicalInfo.Weight);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteUserPhysicalInfo(int id)
        {
            string query = "Delete From UserPhysicalInfo Where InfoID=@ınfoID";
            var parameters = new DynamicParameters();
            parameters.Add("@ınfoID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultUserPhysicalInfoDto>> GetAllUserPhysicalInfoAsync()
        {
            string query = "Select * From UserPhysicalInfo";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultUserPhysicalInfoDto>(query);
                return x.ToList();
            }
        }

		public async Task<UserPhysicalInfoGetByUserIdDto> GetByUserIdAsync(int userId)
		{
			string query = "SELECT InfoID, UserID, Height, Weight FROM UserPhysicalInfo WHERE UserID = @userID";
			var parameters = new DynamicParameters();
			parameters.Add("@userID", userId);
			using (var connection = _context.CreateConnection())
			{
				var x = await connection.QueryFirstOrDefaultAsync<UserPhysicalInfoGetByUserIdDto>(query, parameters);
				return x;
			}
		}

		public async Task<UserPhysicalInfoGetByIDDto> GetInfo(int id)
        {
            string query = "Select * from UserPhysicalInfo Where InfoID=@ınfoID";
            var parameters = new DynamicParameters();
            parameters.Add("@ınfoID", id);
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryFirstOrDefaultAsync<UserPhysicalInfoGetByIDDto>(query, parameters);
                return x;
            }
        }

        public async Task<List<ResultUserPhysicalInfoWithUserDto>> GetResultUserPhysicalInfoWithUserDtos()
        {
            string query = "Select InfoID,Username,Height,Weight From UserPhysicalInfo inner join [User] on UserPhysicalInfo.UserID = [User].UserID";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultUserPhysicalInfoWithUserDto>(query);
                return x.ToList();
            }
        }

        public async void UpdateUserPhysicalInfo(UpdateUserPhysicalInfoDto updateUserPhysicalInfoDto)
        {
            string query = "Update UserPhysicalInfo Set UserID=@userID, Height=@height, Weight=@weight Where InfoID=@ınfoID";
            var parameters = new DynamicParameters();
            parameters.Add("@ınfoID", updateUserPhysicalInfoDto.InfoID);
            parameters.Add("@userID", updateUserPhysicalInfoDto.UserID);
            parameters.Add("@height", updateUserPhysicalInfoDto.Height);
            parameters.Add("@weight", updateUserPhysicalInfoDto.Weight);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
