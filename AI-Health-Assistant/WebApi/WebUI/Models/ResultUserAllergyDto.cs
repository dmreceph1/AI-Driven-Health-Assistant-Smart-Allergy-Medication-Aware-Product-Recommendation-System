namespace WebUI.Models
{
    public class ResultUserAllergyDto
    {
        public int UserAllergyID { get; set; }
        public int AllergyID { get; set; }
        public int UserID { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
