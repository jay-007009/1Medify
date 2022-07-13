using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Pharmacy
{
    public class PharmacySignUpDto
    {
        [Required(ErrorMessage = "This field is required.")]
        [MinLength(6)]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&`~#^()_=+{}[|:;\"'<>.\\]\\\\/-])[A-Za-z\\d@$!%*?&`~#^()_=+{}[|:;\"'<>.\\]\\\\/-]{8,12}$",
            ErrorMessage = "Password must be 8-12 characters long, 1 numeric character, 1 uppercase letter, 1 lowercase letter, & 1 special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]*$",
            ErrorMessage = "Invalid Pharmacy Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$",
            ErrorMessage = "Invalid Date of Establishment.")]
        public string EstablishmentDate { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^[0-9]\d{9}$",
            ErrorMessage = "Invalid Mobile Number.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9]+[\sa-zA-Z0-9\,\.\-\/]*$",
            ErrorMessage = "Invalid Address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Certificate { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string PharmacistDegreeCertificate { get; set; }
    }
}