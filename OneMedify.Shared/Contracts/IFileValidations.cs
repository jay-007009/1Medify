using OneMedify.DTO.Response;

namespace OneMedify.Shared.Contracts
{
    public interface IFileValidations
    {
        ResponseDto ValidateFile(string file, string fileExtension);
        ResponseDto ValidateFileLength(string file);
        ResponseDto UploadFile(string filePath, string file);
    }
}