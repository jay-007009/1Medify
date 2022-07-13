using System.Collections.Generic;

namespace OneMedify.DTO.Patient
{
    public class PatientDetailsDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public decimal? Weight { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public List<string> Diseases { get; set; }
    }
}
