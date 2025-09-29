namespace WebApi.Dtos.UserAllergyDto
{
    public class ResultUserAllergyDetailDto
    {
        public int UserAllergyID { get; set; }
        public int UserID { get; set; }
        public int AllergyID { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
