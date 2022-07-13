using OneMedify.Shared.Contracts;
using System;
using System.IO;

namespace OneMedify.Shared.Services
{
    public class FileService : IFileService
    {
        public string GetFileFromLocation(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                byte[] bytes = File.ReadAllBytes(path);

                var file = Convert.ToBase64String(bytes);

                return (file == String.Empty) ? null : file;

            }
            catch(Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}