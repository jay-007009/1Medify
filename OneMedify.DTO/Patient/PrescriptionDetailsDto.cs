using System.Collections.Generic;

namespace OneMedify.DTO.Patient
{
    public class PrescriptionDetailsDto
    {
        public List<string> Diseases { get; set; }
        public List<PrescriptionMedicineDetailsDto> PrescriptionMedicines { get; set; }
        public string CreatedDate { get; set; }
    }
}
