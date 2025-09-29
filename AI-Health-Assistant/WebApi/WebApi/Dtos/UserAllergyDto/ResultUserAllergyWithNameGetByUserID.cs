namespace WebApi.Dtos.UserAllergyDto
{
    public class ResultUserAllergyWithNameGetByUserID
    {
        public int UserAllergyID { get; set; }
        public string AllergyName { get; set; }
        public int UserID { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
