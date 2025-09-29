using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.MedicationDto;
using WebApi.Dtos.UserAllergyDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Repositories.UserMedicationsRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMedicationsController : ControllerBase
    {
        private readonly IUserMedicationsRepository _userMedicationsRepository;

        public UserMedicationsController(IUserMedicationsRepository userMedicationsRepository)
        {
            _userMedicationsRepository = userMedicationsRepository;
        }
        [HttpGet]
        public async Task<IActionResult> UsermedicationList()
        {
            var values = await _userMedicationsRepository.GetAllUserMedicationsAsync();
            return Ok(values);
        }
        [HttpGet("GetByWithNameAsync")]
        public async Task<IActionResult> GetByWithNameAsync()
        {
            var x = await _userMedicationsRepository.GetByNameAsync();
            return Ok(x);
        }
        [HttpPost]
        public IActionResult CreateUserMedication(CreateUsermedicationDto createUsermedicationDto)
        {
            _userMedicationsRepository.CreateUserMedication(createUsermedicationDto);
            return Ok("başarılı");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUserMedication(int id)
        {
            _userMedicationsRepository.DeleteUserMedication(id);
            return Ok("başarılı");
        }

        [HttpPut]
        public IActionResult UpdateUserMedication(UpdateUserMedicationDto updateUserMedicationDto)
        {
            _userMedicationsRepository.UpdateUserMedication(updateUserMedicationDto);
            return Ok("başarılı");
        }
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserMedicationByUserId(int userId)
        {
            var medication = await _userMedicationsRepository.GetByUserIdAsync(userId);

            if (medication == null)
            {
                return NotFound(new { message = "Kullanıcıya ait ilaç bilgisi bulunamadı." });
            }

            return Ok(medication);
        }

        [HttpGet("getMedicationsByUserIdAndAllergyId")]
        public async Task<IActionResult> GetMedicationsByUserIdAndAllergyId(int userId, int medicationId)
        {
            var hasMedication = await _userMedicationsRepository.CheckUserMedicationAsync(userId, medicationId);
            return Ok(hasMedication);
        }

        [HttpGet("{userId}/medication/{medicationId}")]
        public async Task<IActionResult> GetUserMedicationDetail(int userId, int medicationId)
        {
            var detail = await _userMedicationsRepository.GetUserMedicationDetailAsync(userId, medicationId);
            if (detail == null)
                return NotFound("Kullanıcıya ait ilaç bilgisi bulunamadı");

            return Ok(detail);
        }
    }
}
