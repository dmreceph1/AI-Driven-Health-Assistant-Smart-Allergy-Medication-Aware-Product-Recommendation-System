using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.AllergyDto;
using WebApi.Dtos.ProductDto;
using WebApi.Repositories.AllergyRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyRepository _allergyRepository;

        public AllergyController(IAllergyRepository allergyRepository)
        {
            _allergyRepository = allergyRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAllergy(CreateAllergyDto createAllergyDto)
        {
            _allergyRepository.CreateAllergy(createAllergyDto);
            return Ok("Başarılı");
        }
        [HttpGet]
        public async Task<IActionResult> AllergyList()
        {
            var x = await _allergyRepository.GetAllAllergyAsync();
            return Ok(x);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAllergy(int id)
        {
            _allergyRepository.DeleteAllergy(id);
            return Ok("başarılı");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAllergy(UpdateAllergyDto updateAllergyDto)
        {
            _allergyRepository.UpdateAllergy(updateAllergyDto);
            return Ok("başarılı");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllergy(int id)
        {
            var x = await _allergyRepository.GetAllergy(id);
            return Ok(x);
        }

        [HttpGet("getAllergyByMedicationContent")]
        public async Task<IActionResult> GetAllergyByMedicationContent(string content)
        {
            var allergies = await _allergyRepository.GetAllergyByMedicationContentAsync(content);
            if (allergies == null || allergies.Count == 0)
            {
                return NotFound("Alerji bulunamadı.");
            }

            return Ok(allergies);
        }
    }
}
