namespace WebApi.Dtos.UserDto
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public bool Cinsiyet { get; set; }
    }
}