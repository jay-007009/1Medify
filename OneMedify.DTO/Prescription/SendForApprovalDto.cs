using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Prescription
{
    public class SendForApprovalDto
    {
        [RegularExpression(@"^[0-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
        [StringLength(10)]
        public string PharmacyMobileNumber { get; set; }
    }
}
