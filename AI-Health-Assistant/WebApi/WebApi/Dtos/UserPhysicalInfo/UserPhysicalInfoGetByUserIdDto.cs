namespace WebApi.Dtos.UserPhysicalInfo
{
	public class UserPhysicalInfoGetByUserIdDto
	{
		public int InfoID { get; set; }
		public int UserID { get; set; }
		public decimal Height { get; set; }
		public decimal Weight { get; set; }
	}
}
