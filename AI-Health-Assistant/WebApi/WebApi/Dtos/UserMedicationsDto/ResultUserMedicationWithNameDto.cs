namespace WebApi.Dtos.UserMedicationsDto
{
    public class ResultUserMedicationWithNameDto
    {
        public int UserMedicationID { get; set; }
        public string UserName { get; set; }
        public string MedicationName { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime InactiveDate { get; set; }
    }
}
