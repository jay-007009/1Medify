namespace OneMedify.DTO.Files
{
    public class FileValidationDto
    {
        public string FirstFileExtension { get; set; }
        public string SecondFileExtension { get; set; }
        public int FirstFileLength { get; set; }
        public int SecondFileLength { get; set; }
        public string FirstFileName { get; set; }
        public string SecondFileName { get; set; }
        public string FirstFilePath { get; set; }
        public string SecondFilePath { get; set; }
        public int StatusCode { get; set; }
        public dynamic Response { get; set; }
        public bool IsSucceed { get; set; }
    }
}