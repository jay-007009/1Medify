using System.Collections.Generic;

namespace OneMedify.DTO.Patient
{
    public class PatientProfileDetailsDto
    {
        public string ProfilePicture { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public List<string> Diseases { get; set; }
        public string DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}