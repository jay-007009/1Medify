using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    public partial class Doctor
    {
        
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string InstituteName { get; set; }
        public DateTime InstituteEstablishmentDate { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public string InstituteCertificateFileName { get; set; }
        public string InstituteCertificateFilePath { get; set; }
        public string DoctorDegreeFileName { get; set; }
        public string DoctorDegreeFilePath { get; set; }
        public string ProfilePictureFileName { get; set; }
        public string ProfilePictureFilePath { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName ="smallint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDisable { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Prescription> ModifiedPrescriptions { get; set; }
        public virtual ICollection<DoctorActionLog> DoctorActionLogs { get; set; }
    }
}

