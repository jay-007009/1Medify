using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PrescriptionDetailDto
    {
        public int PrescriptionId { get; set; }
        public string InstituteName { get; set; }
        public string DoctorName { get; set; }
        public string InstituteAddress { get; set; }
        public string DoctorMobileNumber { get; set; }
        public string InstituteCity { get; set; }
        public string InstituteState { get; set; }
        public string CreatedDateTime { get; set; }
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string PatientMobileNumber { get; set; }
        public string Gender { get; set; }
        public double? Weight { get; set; }
        public string Age { get; set; }
        public List<string> Diseases { get; set; }
        public List<MedicineDetailsDto> PrescriptionMedicine { get; set; }
        public string IsExpired { get; set; }
        public string PrescriptionStatus { get; set; }
        public string PrescriptionExpiryDate { get; set; }
        public string ActionTimeStamp { get; set; }
    }
}
