using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PatientPrescriptionDto
    {
        public string PatientName { get; set; }
        public string PatientMobileNumber { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePictureName { get; set; }
        public int PrescriptionId { get; set; }
        public bool PrescriptionType { get; set; }
        public string ActionDateTime { get; set; }
        public List<string> Diseases { get; set; }
        public string IsExpired { get; set; }
        public string PrescriptionStatus { get; set; }
    }
}
