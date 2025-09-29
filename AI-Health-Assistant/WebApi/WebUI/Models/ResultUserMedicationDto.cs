using System;

namespace WebUI.Models
{
    public class ResultUserMedicationDto
    {
        public int UserMedicationID { get; set; }
        public int UserID { get; set; }
        public int MedicationID { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime? InactiveDate { get; set; }
    }
}