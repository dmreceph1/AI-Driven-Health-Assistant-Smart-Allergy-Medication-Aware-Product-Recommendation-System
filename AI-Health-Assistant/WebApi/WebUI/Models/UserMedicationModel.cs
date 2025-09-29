namespace WebUI.Models
{
    public class UserMedicationModel
    {
        public int UserMedicationID { get; set; }
        public int UserID { get; set; }
        public string MedicationName { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime InactiveDate { get; set; }
    }
}
