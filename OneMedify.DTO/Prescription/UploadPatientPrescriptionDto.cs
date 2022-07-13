using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class UploadPatientPrescriptionDto
    {
        public string PharmacyMobileNumber { get; set; }
        public string PatientMobileNumber { get; set; }
        public List<int> DiseaseIds { get; set; }
        public string PrescriptionDocument { get; set; }
    }
}
