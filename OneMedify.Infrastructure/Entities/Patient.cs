using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    public partial class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        [Column(TypeName = "float")]
        public double? Weight { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public string ProfilePictureFileName { get; set; }
        public string ProfilePictureFilePath { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDisable { get; set; }
        public virtual City City { get; set; }
        public virtual IList<PatientDisease> PatientDisease { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Prescription> SentForApprovalPrescriptions { get; set; }
    }
}
