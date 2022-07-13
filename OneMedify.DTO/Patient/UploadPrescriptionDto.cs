using System.Collections.Generic;

namespace OneMedify.DTO.Patient
{
    public class UploadPrescriptionDto
    {
        public string PatientMobileNumber { get; set; }
        public List<int> DiseaseIds { get; set; }
        public string PrescriptionDocument { get; set; }
    }
}
