using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Patient
{
    public class PatientUpdateDto
    {
        public string ProfilePicture { get; set; }
        public decimal? Weight { get; set; }
        [RegularExpression(@"^(?:Male|Other|Female)$", ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9]+[\sa-zA-Z0-9\,\.\-\/]*$", ErrorMessage = "Invalid Address.")]
        public string Address { get; set; }
        public int? CityId { get; set; }
        public bool IsProfilePictureRemoved { get; set; }
    }
}
