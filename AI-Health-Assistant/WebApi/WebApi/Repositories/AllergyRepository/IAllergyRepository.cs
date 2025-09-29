using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.ProductDto;

namespace WebApi.Repositories.AllergyRepository
{
    public interface IAllergyRepository
    {
        Task<List<ResultAllergyDto>> GetAllAllergyAsync();
        void CreateAllergy(CreateAllergyDto createAllergyDto);
        void DeleteAllergy(int id);
        void UpdateAllergy(UpdateAllergyDto updateAllergyDto);
        Task<GetByIDAllergyDto> GetAllergy(int id);
        Task<List<ResultAllergyDto>> GetAllergyByMedicationContentAsync(string content);
    }
}
