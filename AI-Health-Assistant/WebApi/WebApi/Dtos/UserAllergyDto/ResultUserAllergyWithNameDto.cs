namespace WebApi.Dtos.UserAllergyDto
{
    public class ResultUserAllergyWithNameDto
    {
        public int UserAllergyID { get; set; }
        public string AllergyName { get; set; }
        public string UserName { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
