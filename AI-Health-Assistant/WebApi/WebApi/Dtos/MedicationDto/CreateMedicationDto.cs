namespace WebApi.Dtos.MedicationDto
{
    public class CreateMedicationDto
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string ContraindicatedContent { get; set; }
    }
}
