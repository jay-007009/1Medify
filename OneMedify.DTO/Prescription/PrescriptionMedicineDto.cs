using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PrescriptionMedicineDto
    {
        public int MedicineId { get; set; }
        public int MedicineDosage { get; set; }
        public List<string> MedicineTiming { get; set; }
        public bool? AfterBeforeMeal { get; set; }
        public int MedicineDays { get; set; }
    }
}
