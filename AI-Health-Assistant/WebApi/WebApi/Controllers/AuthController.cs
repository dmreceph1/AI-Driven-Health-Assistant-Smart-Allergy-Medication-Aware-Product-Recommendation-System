using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebApi.Dtos.UserDto;
using WebApi.Repositories.UserRepository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;

        public AuthController(IUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAndPassword(loginDto.UserName, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token, UserId = user.UserID });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto registerDto)
        {
            var existingUser = await _userRepository.GetUser(registerDto.UserName);
            if (existingUser != null)
            {
                return BadRequest("Bu kullanıcı adı zaten mevcut.");
            }

            
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            var passwordSalt = hmac.Key;

            _userRepository.CreateUser(registerDto);

            var newUser = await _userRepository.GetUser(registerDto.UserName);
            var token = _authService.GenerateJwtToken(newUser);

            return Ok(new { Token = token });
        }
    }
}