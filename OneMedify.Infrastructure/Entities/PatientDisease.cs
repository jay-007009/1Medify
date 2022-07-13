using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("PatientDisease", Schema = "Patient")]
    public partial class PatientDisease
    {
        [Column(TypeName = "smallint")]
        public int PatientDiseaseId { get; set; }

        [Column(TypeName = "smallint")]
        public int PatientId { get; set; }

        [Column(TypeName = "tinyint")]
        public int DiseaseId { get; set; }

        [Column(TypeName = "smallint")]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "smallint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDisable { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
