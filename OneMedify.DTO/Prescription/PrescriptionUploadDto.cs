using System.Collections.Generic;

namespace OneMedify.DTO.Prescription
{
    public class PrescriptionUploadDto
    {
        public int PrescriptionId { get; set; }
        public List<string>  Diseases { get; set; }  
        public string PrescriptionExpiryDate { get; set; }
        public string PrescriptionStatus {get; set;}
        public string PrescriptionFile { get; set; }
        public string PrescriptionFileName { get; set; }
        public string PrescriptionFilePath { get; set; }
        public bool PrescriptionType { get; set; }
        public string IsExpired { get; set; }
        public string ActionTimeStamp { get; set; }
    }
}
