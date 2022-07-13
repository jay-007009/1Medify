using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("DoctorActionLog", Schema = "Doctor")]
    public class DoctorActionLog
    {
        [Key]
        public int DoctorActionLogId { get; set; }
       
        public int DoctorId { get; set; }
        
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public int PrescriptionId { get; set; }

        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }
        
        public int PrescriptionStatus { get; set; }
        public DateTime ActionTimeStamp { get; set; }
        public string DoctorList { get; set; }        
        
    }
}
