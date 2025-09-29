using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.UserAllergyDto;
using WebApi.Dtos.UserMedicationsDto;
using WebApi.Repositories.UserAllergyRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAllergyController : ControllerBase
    {
        private readonly IUserAllergyRepository _userAllergyRepository;

        public UserAllergyController(IUserAllergyRepository userAllergyRepository)
        {
            _userAllergyRepository = userAllergyRepository;
        }
        [HttpGet]
        public async Task<IActionResult> UserAllergyList()
        {
            var values = await _userAllergyRepository.GetAllUserAllergyAsync();
            return Ok(values);
        }
        [HttpGet("GetByWithNameAsync")]
        public async Task<IActionResult> GetByWithNameAsync()
        {
            var x = await _userAllergyRepository.GetByNameAsync();
            return Ok(x);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserAllergy(CreateUserAllergyDto createUserAllergyDto)
        {
            var result = await _userAllergyRepository.CreateUserAllergy(createUserAllergyDto);
            if (result)
                return Ok("Kullanıcı alerjisi başarıyla eklendi");
            return BadRequest("Kullanıcı alerjisi eklenirken bir hata oluştu");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUserAllergy(int id)
        {
            _userAllergyRepository.DeleteUserAllergy(id);
            return Ok("Kullanıcı alerjisi başarıyla silindi");
        }

        [HttpPut]
        public IActionResult UpdateUserAllergy(UpdateUserAllergyDto updateUserAllergyDto)
        {
            _userAllergyRepository.UpdateUserAllergy(updateUserAllergyDto);
            return Ok("Kullanıcı alerjisi başarıyla güncellendi");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(int userId)
        {
            var allergy = await _userAllergyRepository.GetByUserIdAsync(userId);

            if (allergy == null)
            {
                return NotFound(new { message = "Kullanıcıya ait alerji bilgisi bulunamadı" });
            }

            return Ok(allergy);
        }

        [HttpGet("getAllergiesByUserIdAndAllergyId")]
        public async Task<IActionResult> GetAllergiesByUserIdAndAllergyId(int userId, int allergyId)
        {
            var hasAllergy = await _userAllergyRepository.CheckUserAllergyAsync(userId, allergyId);
            return Ok(hasAllergy);
        }

        [HttpGet("{userId}/allergy/{allergyId}")]
        public async Task<IActionResult> GetUserAllergyDetail(int userId, int allergyId)
        {
            var detail = await _userAllergyRepository.GetUserAllergyDetailAsync(userId, allergyId);
            if (detail == null)
                return NotFound("Kullanıcıya ait alerji bilgisi bulunamadı");

            return Ok(detail);
        }

    }
}
