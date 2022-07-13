using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Patient
{
    public class PatientSignUpDto
    {
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(150, MinimumLength = 6)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&`~#^()_=+{}[|:;\"'<>.\\]\\\\/-])[A-Za-z\\d@$!%*?&`~#^()_=+{}[|:;\"'<>.\\]\\\\/-]{8,12}$",
            ErrorMessage = "Password must be 8-12 characters long, 1 numeric character, 1 uppercase letter, 1 lowercase letter, & 1 special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(48, MinimumLength = 2)]
        [RegularExpression("[A-Za-z]*$", ErrorMessage = "Invalid First Name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(48, MinimumLength = 2)]
        [RegularExpression("[A-Za-z]*$", ErrorMessage = "Invalid Last Name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string DateOfBirth { get; set; }

        [RegularExpression(@"^\d{1,3}(\.\d{0,2})*$", ErrorMessage = "Invalid weight.")]
        [Range(1, 501)]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^[0-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
        [StringLength(10)]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^(?:Male|Other|Female)", ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9]+[\sa-zA-Z0-9\,\.\-\/]*$", ErrorMessage = "Invalid Address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int CityId { get; set; }

        public List<int> DiseaseIds { get; set; }
    }
}
