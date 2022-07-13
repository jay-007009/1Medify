namespace OneMedify.DTO.Doctor
{
    public class FileUploadDto
    {
        public string DegreeCertificateFileName { get; set; }
        public string DegreeCertificateFileExtension { get; set; }
        public string DegreeCertificateFilePath { get; set; }
        public int DegreeCertificateFileLength { get; set; }
        public string InstituteCertificateFileName { get; set; }
        public string InstituteCertificateFileExtension { get; set; }
        public string InstituteCertificateFilePath { get; set; }
        public int InstituteCertificateFileLength { get; set; }
        public bool IsValid { get; set; }
        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}