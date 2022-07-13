using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class ApprovedCreatedPrescriptionsDto
    {
        public int PrescriptionId { get; set; }
        public List<string> Diseases { get; set; }

        public string DoctorName { get; set; }

        public string IsExpired { get; set; }

        public string ActionDateTime { get; set; }
    }
}
