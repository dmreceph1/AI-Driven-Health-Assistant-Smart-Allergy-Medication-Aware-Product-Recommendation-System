namespace WebApi.Dtos.MedicationDto
{
    public class GetByIDMedicationDto
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string ContraindicatedContent { get; set; }
    }
}
