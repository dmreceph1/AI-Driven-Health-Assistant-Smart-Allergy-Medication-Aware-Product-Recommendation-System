namespace WebApi.Dtos.UserPhysicalInfo
{
    public class CreateUserPhysicalInfoDto
    {
        public int UserID { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }
}
