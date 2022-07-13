using OneMedify.Shared.Contracts;
using System;
using System.IO;

namespace OneMedify.Shared.Services
{
    public class FileUpload : IFileUpload

    {
        /// <summary>
        /// Method for Validating Extension of Document
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetExtensionOfDocument(string file)
        {
            if (file.Length >= 5)
            {
                var extension = file.Substring(0, 5);
                return extension switch
                {
                    "iVBOR" => ".png",
                    "JVBER" => ".pdf",
                    "/9j/4" => ".jpeg",
                    "Qk2Ke" => ".bmp",
                    "SUkqA" => ".tiff",
                    _ => string.Empty,
                };
            }
            return string.Empty;
        }

        /// <summary>
        /// Method for Validating Extension of Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetExtensionOfImage(string file)
        {
            if (file.Length >= 5)
            {
                var extension = file.Substring(0, 5);
                return extension switch
                {
                    "iVBOR" => ".png",
                    "/9j/4" => ".jpeg",
                    "Qk2Ke" => ".bmp",
                    "SUkqA" => ".tiff",
                    _ => string.Empty,
                };
            }
            return string.Empty;
        }

        /// <summary>
        /// Method for Getting File Path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFilePath(string fileName)
        {
            return Path.Combine(@"C:\StaticFiles", fileName);
        }

        /// <summary>
        /// Method for Getting File Name
        /// </summary>
        /// <param name="email"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public string GetFileName(string email, string fileExtension)
        {
            var name = email.Split("@")[0];
            return GetName(name) + Guid.NewGuid().ToString() + fileExtension;
        }

        /// <summary>
        /// Method for Uploading File
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        public async void UploadFile(string filePath, string file)
        {
            try
            {
                byte[] fileByte = Convert.FromBase64String(file);
                await File.WriteAllBytesAsync(filePath, fileByte);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Getting Length of File
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int GetLengthOfFile(string file)
        {
            return Convert.FromBase64String(file).Length;
        }

        /// <summary>
        /// Method for getting Name for File Based on Length of User's Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetName(string name)
        {
            if (name.Length >= 10)
            {
                return name.Substring(0, 10);
            }
            else
            {
                return name.Length switch
                {
                    9 => name.Substring(0, 9),
                    8 => name.Substring(0, 8),
                    7 => name.Substring(0, 7),
                    6 => name.Substring(0, 6),
                    5 => name.Substring(0, 5),
                    4 => name.Substring(0, 4),
                    3 => name.Substring(0, 3),
                    2 => name.Substring(0, 2),
                    _ => name.Substring(0, 1),
                };
            }
        }

        /// <summary>
        /// Method to delete file from specified location
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool DeleteFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath) || filePath == null)
                {
                    return true;
                }
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPrescriptionFileName(string Name, string fileExtension)
        {
            return Name.Split("@")[0] + "'s Prescription" + fileExtension;
        }
    }
}