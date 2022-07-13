using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("Prescription", Schema = "Prescription")]
    public class Prescription
    {
        [Key]
        [Column("PrescriptionID")]
        public int PrescriptionId { get; set; }

        [Column(TypeName = "tinyint")]
        public int PrescriptionStatus { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime? ExpiryDate { get; set; }

        [Column(TypeName = "smallint")]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedByDoctor { get; set; }
        public virtual Doctor ApprovedByDoctor { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedByPatient { get; set; }
        public virtual Patient SentFromPatient { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedByPharmacy { get; set; }
        public virtual Pharmacy SentFromPharmacy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDisable { get; set; }
      
        public bool PrescriptionType { get; set; }

        [Column("DoctorID", TypeName = "smallint")]
        public int? DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [Column("PatientID", TypeName = "smallint")]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Column("PharmacyID", TypeName = "smallint")]
        public int? PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; }
        public virtual PrescriptionUpload PrescriptionUpload { get; set; }
        public virtual ICollection<DoctorActionLog> DoctorActionLogs { get; set; }
    }
}
