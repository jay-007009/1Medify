using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("Medicine", Schema = "Prescription")]
    public class Medicine
    {
        [Key]
        [Column("MedicineID")]
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }

        [Column(TypeName = "smallint")]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDisable { get; set; }

        [Column("DiseaseID", TypeName = "tinyint")]
        public int DiseaseId { get; set; }
        public virtual Disease Disease { get; set; }

        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; }
    }
}
