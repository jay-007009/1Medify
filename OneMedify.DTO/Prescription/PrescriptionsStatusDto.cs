using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PrescriptionsStatusDto
    {
        public int PrescriptionId { get; set; }
        public string DoctorName { get; set; }
        public string PrescriptionStatus { get; set; }
        public string ActionDateTime { get; set; }
        public List<string> Diseases { get; set; }
    }
}
