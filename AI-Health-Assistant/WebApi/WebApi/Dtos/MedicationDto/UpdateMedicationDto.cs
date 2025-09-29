namespace WebApi.Dtos.MedicationDto
{
    public class UpdateMedicationDto
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string ContraindicatedContent { get; set; }
    }
}
