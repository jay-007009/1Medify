using System.ComponentModel.DataAnnotations.Schema;

namespace OneMedify.Infrastructure.Entities
{
    [Table("PrescriptionUpload", Schema = "Prescription")]
    public class PrescriptionUpload
    {
        [Column("PrescriptionUploadID")]
        public int PrescriptionUploadId { get; set; }
        public string PrescriptionFileName { get; set; }
        public string PrescriptionFilePath { get; set; }
        public bool? IsDisable { get; set; }        
        public string Diseases { get; set; }

        [Column("PrescriptionID", TypeName = "smallint")]
        public int PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }
    }
}