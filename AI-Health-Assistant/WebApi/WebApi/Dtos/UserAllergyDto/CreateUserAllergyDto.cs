namespace WebApi.Dtos.UserAllergyDto
{
    public class CreateUserAllergyDto
    {
        public int AllergyID { get; set; }
        public int UserID { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
