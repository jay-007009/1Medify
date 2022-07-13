using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Shared.Contracts;
using System;

namespace OneMedify.Shared.Services
{
    public class FileValidations : IFileValidations
    {
        private readonly IFileUpload _fileUpload;

        public FileValidations(IFileUpload fileUpload)
        {
            _fileUpload = fileUpload;
        }

        /// <summary>
        /// Method for Validating Length of File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        public ResponseDto ValidateFileLength(string file)
        {
            var fileLength = _fileUpload.GetLengthOfFile(file);
            if (fileLength > 3000000)
            {
                return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidFileSize };
            }
            return new ResponseDto { StatusCode = 200, Response = DoctorResources.FileValidationSuccess };
        }

        /// <summary>
        /// Method for Uploading File
        /// </summary>
        /// <param name="email"></param>
        /// <param name="fileValidationDto"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public ResponseDto UploadFile(string filePath, string file)
        {
            try
            {
                _fileUpload.UploadFile(filePath, file);
                return new ResponseDto { StatusCode = 200, Response = DoctorResources.FileUploaded };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }

        public ResponseDto ValidateFile(string file, string fileExtension)
        {
            try
            {
                if (fileExtension == string.Empty)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidFileExtension };
                }
                var ValidLength = ValidateFileLength(file);
                if (ValidLength.StatusCode != 200)
                {
                    return ValidLength;
                }
                return ValidLength;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}