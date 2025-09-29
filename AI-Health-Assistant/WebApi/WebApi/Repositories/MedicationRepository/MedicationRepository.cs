using Dapper;
using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.MedicationDto;
using WebApi.Dtos.ProductDto;
using WebApi.Models.PContext;

namespace WebApi.Repositories.MedicationRepository
{
    public class MedicationRepository:IMedicationRepository
    {
        private readonly Context _context;

        public MedicationRepository(Context context)
        {
            _context = context;
        }

        public async void CreateMedication(CreateMedicationDto createMedicationDto)
        {
            string query = "insert into Medications (MedicationName, Dosage, ContraindicatedContent) Values (@medicationName, @dosage, @contraindicatedContent)";
            var parameters = new DynamicParameters();
            parameters.Add("@medicationName", createMedicationDto.MedicationName);
            parameters.Add("@dosage", createMedicationDto.Dosage);
            parameters.Add("@contraindicatedContent", createMedicationDto.ContraindicatedContent);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async void DeleteMedication(int id)
        {
            string query = "Delete From Medications Where MedicationID=@medicationID";
            var parameters = new DynamicParameters();
            parameters.Add("@medicationID", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultMedicationDto>> GetAllMedicationAsync()
        {
            string query = "Select*From Medications";
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryAsync<ResultMedicationDto>(query);
                return x.ToList();
            }
        }

        public async Task<GetByIDMedicationDto> GetMedication(int id)
        {
            string query = "Select * from Medications Where MedicationID=@medicationID";
            var parameters = new DynamicParameters();
            parameters.Add("@medicationID", id);
            using (var connection = _context.CreateConnection())
            {
                var x = await connection.QueryFirstOrDefaultAsync<GetByIDMedicationDto>(query, parameters);
                return x;
            }
        }

        public async void UpdateMedication(UpdateMedicationDto updateMedicationDto)
        {
            string query = "Update Medications Set MedicationName=@medicationName,Dosage=@dosage,ContraindicatedContent=@contraindicatedContent Where MedicationID=@medicationID";
            var parameters = new DynamicParameters();
            parameters.Add("@medicationName", updateMedicationDto.MedicationName);
            parameters.Add("@dosage", updateMedicationDto.Dosage);
            parameters.Add("@contraindicatedContent", updateMedicationDto.ContraindicatedContent);
            parameters.Add("@medicationID", updateMedicationDto.MedicationID);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultMedicationDto>> GetAllergyByIlacContentAsync(string content)
        {
            string query = "SELECT * FROM Medications WHERE ContraindicatedContent LIKE @content";
            var parameters = new DynamicParameters();
            parameters.Add("@content", "%" + content + "%");
            using (var connection = _context.CreateConnection())
            {
                var medications = await connection.QueryAsync<ResultMedicationDto>(query, parameters);
                return medications.ToList();
            }
        }
    }
}
