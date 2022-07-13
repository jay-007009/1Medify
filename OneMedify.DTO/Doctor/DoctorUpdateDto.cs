using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Doctor
{
    public class DoctorUpdateDto
    {
        public string ProfilePicture { get; set; }

        [RegularExpression(@"^(?:Male|Other|Female)$", ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([a-zA-Z]+\s?)*[^\s]$", ErrorMessage = "Invalid Specialization.")]
        public string Specialization { get; set; }

        public string Degreecertificate { get; set; }

        public bool IsProfilePictureRemoved { get; set; }
    }
}