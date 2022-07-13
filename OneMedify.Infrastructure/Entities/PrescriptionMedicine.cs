using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("PrescriptionMedicine", Schema = "Prescription")]
    public class PrescriptionMedicine
    {
        [Key]
        [Column("PrescriptionMedicineID")]
        public int PrescriptionMedicineId { get; set; }

        [Column(TypeName = "tinyint")]
        public int MedicineDosage { get; set; }
        public string MedicineTiming { get; set; }
        public bool AfterBeforeMeal { get; set; }

        [Column(TypeName = "tinyint")]
        public int MedicineDays { get; set; }

        [Column(TypeName = "smallint")]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDisable { get; set; }

        [Column("MedicineID", TypeName = "tinyint")]
        public int MedicineId { get; set; }
        public virtual Medicine Medicine { get; set; }

        [Column("PrescriptionID", TypeName = "smallint")]
        public int PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }
    }
}
