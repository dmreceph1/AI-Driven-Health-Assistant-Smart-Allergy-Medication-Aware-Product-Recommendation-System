namespace WebApi.Dtos.UserMedicationsDto
{
    public class CreateUsermedicationDto
    {
        public int UserID { get; set; }
        public int MedicationID { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime? InactiveDate { get; set; }
    }
}
