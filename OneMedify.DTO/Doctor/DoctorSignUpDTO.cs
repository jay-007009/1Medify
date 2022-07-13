using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Doctor
{
    public class DoctorSignUpDto
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
        [RegularExpression("^([a-zA-Z]+\\s?)*[^\\s]$", ErrorMessage = "Invalid First Name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(48, MinimumLength = 2)]
        [RegularExpression("^([a-zA-Z]+\\s?)*[^\\s]$", ErrorMessage = "Invalid Last Name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^([a-zA-Z]+\\s?)*[^\\s]$", ErrorMessage = "Invalid Specialization.")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^[0-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(100)]
        [RegularExpression("^([a-zA-Z0-9]+\\s?)*[^\\s]$", ErrorMessage = "Invalid Institute Name.")]
        public string InstituteName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$", ErrorMessage = "Invalid Date of Establishment.")]
        public string InstituteEstablishmentDate { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^(?:Male|Other|Female)", ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9]+[\sa-zA-Z0-9\,\.\-\/]*$", ErrorMessage = "Invalid Address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string DegreeCertificate { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string InstituteCertificate { get; set; }
    }
}