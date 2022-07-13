namespace OneMedify.Shared.Contracts
{
    public interface IFileUpload
    {
        string GetExtensionOfDocument(string file);
        string GetExtensionOfImage(string file);
        string GetFileName(string email, string fileExtension);
        string GetFilePath(string fileName);
        void UploadFile(string filePath, string file);
        int GetLengthOfFile(string file);
        bool DeleteFile(string filePath);
        string GetPrescriptionFileName(string Name, string fileExtension);
    }
}