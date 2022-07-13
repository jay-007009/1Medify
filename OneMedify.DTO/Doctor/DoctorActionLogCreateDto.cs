using System;

namespace OneMedify.DTO.Doctor
{
    public class DoctorActionLogCreateDto
    {
        public int DoctorId { get; set; }
        public int PrescriptionId { get; set; }
        public int PrescriptionStatus { get; set; }
        public DateTime ActionTimeStamp { get; set; }
        public string DoctorList { get; set; }
    }
}
