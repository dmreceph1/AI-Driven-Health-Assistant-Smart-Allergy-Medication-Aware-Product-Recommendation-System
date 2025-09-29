namespace WebApi.Dtos.UserPhysicalInfo
{
    public class UserPhysicalInfoGetByIDDto
    {
        public int InfoID { get; set; }
        public int UserID { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }
}
