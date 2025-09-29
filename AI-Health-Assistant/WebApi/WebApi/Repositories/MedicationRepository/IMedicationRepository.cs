using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.MedicationDto;
using WebApi.Dtos.ProductDto;

namespace WebApi.Repositories.MedicationRepository
{
    public interface IMedicationRepository
    {
        Task<List<ResultMedicationDto>> GetAllMedicationAsync();
        void CreateMedication(CreateMedicationDto createMedicationDto);
        void DeleteMedication(int id);
        void UpdateMedication(UpdateMedicationDto updateMedicationDto);
        Task<GetByIDMedicationDto> GetMedication(int id);
        Task<List<ResultMedicationDto>> GetAllergyByIlacContentAsync(string content);
    }
}

