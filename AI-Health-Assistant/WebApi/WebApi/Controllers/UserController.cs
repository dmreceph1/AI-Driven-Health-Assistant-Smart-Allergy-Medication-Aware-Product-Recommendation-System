using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.UserDto;
using WebApi.Repositories.UserRepository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {        
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            _userRepository.CreateUser(createUserDto);
            return Ok("Başarılı");
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await _userRepository.GetAllUserAsync();
            return Ok(users);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
            return Ok("başarılı");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            _userRepository.UpdateUser(updateUserDto);
            return Ok("başarılı");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            return Ok(user);
        }
    }
}