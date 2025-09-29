using Dapper;
using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.ProductDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.AllergyRepository
{
    public class AllergyRepository : IAllergyRepository
    {
        private readonly Context _context;

        public AllergyRepository(Context context)
        {
            _context = context;
        }

        public async void CreateAllergy(CreateAllergyDto createAllergyDto)
        {
            string query = "insert into Allergy (AllergyName, MedicationContent) Values (@allergyName, @medicationContent)";
            var parameters = new DynamicParameters();
            parameters.Add("@allergyName", createAllergyDto.AllergyName);
            parameters.Add("@medicationContent", createAllergyDto.MedicationContent);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteAllergy(int id)
        {
            string query = "Delete From Allergy Where AllergyID=@allergyID";
            var parameters = new DynamicParameters();
            parameters.Add("@allergyID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultAllergyDto>> GetAllAllergyAsync()
        {
            string query = "Select*From Allergy";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultAllergyDto>(query);
                return x.ToList();
            }
        }

        public async Task<GetByIDAllergyDto> GetAllergy(int id)
        {
            string query = "Select * from Allergy Where AllergyID=@allergyID";
            var parameters = new DynamicParameters();
            parameters.Add("@allergyID", id);
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryFirstOrDefaultAsync<GetByIDAllergyDto>(query, parameters);
                return x;
            }
        }

        public async void UpdateAllergy(UpdateAllergyDto updateAllergyDto)
        {
            string query = "Update allergy Set AllergyName=@allergyName,MedicationContent=@medicationContent Where AllergyID=@allergyID";
            var parameters = new DynamicParameters();
            parameters.Add("@allergyName", updateAllergyDto.AllergyName);
            parameters.Add("@medicationContent", updateAllergyDto.MedicationContent);
            parameters.Add("@allergyID", updateAllergyDto.AllergyID);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultAllergyDto>> GetAllergyByMedicationContentAsync(string content)
        {
            string query = "SELECT * FROM Allergy WHERE MedicationContent LIKE @content";
            var parameters = new DynamicParameters();
            parameters.Add("@content", "%" + content + "%");
            using (var connection = _context.CreateConnection())
            {
                var allergies = await connection.QueryAsync<ResultAllergyDto>(query, parameters);
                return allergies.ToList();
            }
        }
    }
}
