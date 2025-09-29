using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.UserAllergyDto;
using WebApi.Dtos.UserPhysicalInfo;
using WebApi.Repositories.ProductContentsRepository;
using WebApi.Repositories.UserPhysicalInfoRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPhysicalInfoController : ControllerBase
    {
        private readonly IUserPhysicalInfoRepository _userPhysicalInfoRepository;

        public UserPhysicalInfoController(IUserPhysicalInfoRepository userPhysicalInfoRepository)
        {
            _userPhysicalInfoRepository = userPhysicalInfoRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ProductUserPhysicalInfoList()
        {
            var x = await _userPhysicalInfoRepository.GetAllUserPhysicalInfoAsync();
            return Ok(x);
        }
        [HttpGet("UserPhysicalInfoWithUser")]
        public async Task<IActionResult> UserPhysicalInfoWithUser()
        {
            var x = await _userPhysicalInfoRepository.GetResultUserPhysicalInfoWithUserDtos();
            return Ok(x);
        }
        [HttpPost]
        public IActionResult CreateUserPhysicalInfo(CreateUserPhysicalInfoDto createUserPhysicalInfo)
        {
            _userPhysicalInfoRepository.CreateUserPhysicalInfo(createUserPhysicalInfo);
            return Ok("başarılı");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUserPhysicalInfo(int id)
        {
            _userPhysicalInfoRepository.DeleteUserPhysicalInfo(id);
            return Ok("başarılı");
        }

        [HttpPut]
        public IActionResult UpdateUserPhysicalInfo(UpdateUserPhysicalInfoDto updateUserPhysicalInfo)
        {
            _userPhysicalInfoRepository.UpdateUserPhysicalInfo(updateUserPhysicalInfo);
            return Ok("başarılı");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfo(int id)
        {
            var x = await _userPhysicalInfoRepository.GetInfo(id);
            return Ok(x);
        }
		[HttpGet("user/{userid}")]
		public async Task<IActionResult> GetByUserIdAsync(int userid)
		{
			var x = await _userPhysicalInfoRepository.GetByUserIdAsync(userid);
			return Ok(x);
		}
	}
}
