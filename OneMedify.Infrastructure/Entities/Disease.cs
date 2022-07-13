using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("Disease", Schema = "Patient")]
    public partial class Disease
    {
        public Disease()
        {
            PatientDisease = new HashSet<PatientDisease>();
        }

        [Column(TypeName = "tinyint")]
        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }

        [Column(TypeName = "tinyint")]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "tinyint")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDisable { get; set; }

        public virtual ICollection<PatientDisease> PatientDisease { get; set; }

        public virtual ICollection<Medicine> Medicines { get; set; }
    }
}
