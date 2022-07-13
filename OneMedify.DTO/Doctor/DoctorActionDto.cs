using System.ComponentModel.DataAnnotations;

namespace OneMedify.DTO.Doctor
{
    public class DoctorActionDto
    {
        public string DoctorMobileNumber { get; set; }

        [RegularExpression(@"^(?:Approve|Reject|Busy)", ErrorMessage = "Invalid Action.")]
        public string Action { get; set; }
        public string ExpiryDate { get; set; }
    }
}
