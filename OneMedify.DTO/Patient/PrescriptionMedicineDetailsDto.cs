namespace OneMedify.DTO.Patient
{
   public class PrescriptionMedicineDetailsDto
    {
        public string MedicineName { get; set; }
        public int MedicineDosage { get; set; }
        public string MedicineTiming { get; set; }
        public bool AfterBeforeMeal { get; set; }
        public int MedicineDays { get; set; }
    }
}
