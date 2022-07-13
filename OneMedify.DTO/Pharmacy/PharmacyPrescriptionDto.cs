using System.Collections.Generic;

namespace OneMedify.DTO.Pharmacy
{
    public class PharmacyPrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePictureName { get; set; }
        public string PatientName { get; set; }
        public string PrescriptionStatus { get; set; }
        public List<string> Diseases { get; set; }
        public string DoctorName { get; set; }
        public string ActionDateTime { get; set; }
    }
}
