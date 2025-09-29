namespace WebApi.Dtos.UserMedicationsDto
{
    public class ResultUserMedicationDetailDto
    {
        public int UserMedicationID { get; set; }
        public int UserID { get; set; }
        public int MedicationID { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime? InactiveDate { get; set; }
    }
}
