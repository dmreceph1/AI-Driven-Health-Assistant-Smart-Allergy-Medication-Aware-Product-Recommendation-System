namespace WebApi.Dtos.UserDto
{
    public class UpdateUserDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public bool Cinsiyet { get; set; }
    }
}