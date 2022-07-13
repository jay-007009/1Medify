using System.Text.Json.Serialization;

namespace OneMedify.DTO.DoctorActionLog
{
    public class PrescriptionByPharmacyDto
    {
        public int PrescriptionId { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePictureName { get; set; }
        public string PatientName { get; set; }
        public string PrescriptionStatus { get; set; }
        public bool PrescriptionType { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string PharmacyName { get; set; }
        public string ActionDateTime { get; set; }

        [JsonIgnore]
        public int PharmacyId { get; set; }
        [JsonIgnore]
        public int ModifiedByPharmacy { get; set; }

    }
}
