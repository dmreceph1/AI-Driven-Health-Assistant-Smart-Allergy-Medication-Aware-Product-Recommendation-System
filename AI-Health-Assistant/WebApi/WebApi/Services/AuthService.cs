using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Dtos.UserDto;

namespace WebApi.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(ResultUserDto user) // kullanýcý bilgilerini alýp token oluþturuyor
        {
            var claims = new List<Claim>  // Token içine koyulacak bilgiler
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); // Gizli anahtar
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  // Ýmzalama algoritmasý
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],   // Token’ý kim oluþturdu
                _config["Jwt:Audience"],  // Token’ý kim kullanacak
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpirationInMinutes"])), // Token geçerlilik süresi
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}