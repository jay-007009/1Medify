using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PharmacyPrescriptionDto
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string PharmacyMobileNumber { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePictureName { get; set; }
        public int PrescriptionId { get; set; }
        public string PrescriptionCreatedDate { get; set; }
        public List<string> Diseases { get; set; }
        public string IsExpired { get; set; }
        public string PrescriptionStatus { get; set; }
    }
}
