using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.MedicationDto;
using WebApi.Dtos.ProductDto;
using WebApi.Repositories.MedicationRepository;
using WebApi.Repositories.ProductRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationRepository _medicationRepository;

        public MedicationController(IMedicationRepository medicationRepository)
        {
            _medicationRepository = medicationRepository;
        }
        [HttpGet]
        public async Task<IActionResult> MedicationList()
        {
            var x = await _medicationRepository.GetAllMedicationAsync();
            return Ok(x);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMedication(CreateMedicationDto createMedicationDto)
        {
            _medicationRepository.CreateMedication(createMedicationDto);
            return Ok("Başarılı");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            _medicationRepository.DeleteMedication(id);
            return Ok("başarılı");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMedication(UpdateMedicationDto updateMedicationDto)
        {
            _medicationRepository.UpdateMedication(updateMedicationDto);
            return Ok("başarılı");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedication(int id)
        {
            var x = await _medicationRepository.GetMedication(id);
            return Ok(x);
        }

        [HttpGet("getMedicationByIlacContent")]
        public async Task<IActionResult> GetMedicationByIlacContent(string content)
        {
            var medi = await _medicationRepository.GetAllergyByIlacContentAsync(content);
            if (medi == null || medi.Count == 0)
            {
                return NotFound("İlaç bulunamadı.");
            }

            return Ok(medi);
        }
    }
}
