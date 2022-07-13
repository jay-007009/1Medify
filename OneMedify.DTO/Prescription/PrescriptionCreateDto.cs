using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PrescriptionCreateDto
    {
        public List<int> DiseaseIds { get; set; }
        public string DoctorMobileNumber { get; set; }
        public string PatientMobileNumber { get; set; }
        public string ExpiryDate { get; set; }
        public List<PrescriptionMedicineDto> Medicines { get; set; }
    }
}
