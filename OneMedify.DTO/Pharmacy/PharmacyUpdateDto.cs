using System.ComponentModel.DataAnnotations;
namespace OneMedify.DTO.Pharmacy
{
    public class PharmacyUpdateDto
    {
        public string ProfilePicture { get; set; }
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9]+[\sa-zA-Z0-9\,\.\-\/]*$", ErrorMessage = "Invalid Address.")]
        public string Address { get; set; }
        public int? CityId { get; set; }
        public string PharmacistDegreeCertificate { get; set; }
        public bool IsProfilePictureRemoved { get; set; }
    }
}
