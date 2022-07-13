using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("Pharmacy", Schema = "Pharmacy")]
    public partial class Pharmacy
    {
        [Key]
        [Column("PharmacyID")]
        public int? PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public DateTime? PharmacyEstablishmentDate { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "smallint")]
        public int? CityId { get; set; }
        public string PharmacyCertificateFileName { get; set; }
        public string PharmacyCertificateFilePath { get; set; }
        public string PharmacistDegreeFileName { get; set; }
        public string PharmacistDegreeFilePath { get; set; }
        public string ProfilePictureFileName { get; set; }
        public string ProfilePictureFilePath { get; set; }
        [Column(TypeName = "smallint")]
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "smallint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDisable { get; set; }
        public virtual City City { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; }

        public virtual ICollection<Prescription> SentForApprovalPrescriptions { get; set; }

    }
}