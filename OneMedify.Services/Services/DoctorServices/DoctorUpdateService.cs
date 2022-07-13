using OneMedify.DTO.Doctor;
using OneMedify.DTO.Files;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class DoctorUpdateService : IDoctorUpdateService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IFileUpload _fileUpload;
        private readonly IFileValidations _fileValidations;

        public DoctorUpdateService(IDoctorRepository doctorRepository, IFileUpload fileUpload, IFileValidations fileValidations)
        {
            _doctorRepository = doctorRepository;
            _fileUpload = fileUpload;
            _fileValidations = fileValidations;
        }

        /// <summary>
        /// Method for Update Doctor by Mobile Number
        /// </summary>    
        /// <param name="mobileNumber"></param>
        /// <param name="doctorUpdateDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UpdateDoctorAsync(string mobileNumber, DoctorUpdateDto doctorUpdateDto)
        {
            try
            {
                if (doctorUpdateDto.Specialization == null && doctorUpdateDto.Degreecertificate == null && doctorUpdateDto.Gender == null && doctorUpdateDto.ProfilePicture == null)
                {
                    return new ResponseDto { StatusCode = 200, Response = DoctorResources.NothingToUpdate };
                }
                FileValidationDto fileValidationDto = new FileValidationDto();
                var doctor = await _doctorRepository.GetDoctorAsync(mobileNumber);
                if (doctorUpdateDto.Degreecertificate != null)
                {
                    fileValidationDto.FirstFileExtension = _fileUpload.GetExtensionOfDocument(doctorUpdateDto.Degreecertificate);
                    var isFileValid = _fileValidations.ValidateFile(doctorUpdateDto.Degreecertificate, fileValidationDto.FirstFileExtension);
                    if (isFileValid.StatusCode != 200)
                    {
                        return isFileValid;
                    }
                }
                if (doctorUpdateDto.IsProfilePictureRemoved && doctorUpdateDto.ProfilePicture != null)
                {
                    fileValidationDto.SecondFileExtension = _fileUpload.GetExtensionOfImage(doctorUpdateDto.ProfilePicture);
                    var isFileValid = _fileValidations.ValidateFile(doctorUpdateDto.ProfilePicture, fileValidationDto.SecondFileExtension);
                    if (isFileValid.StatusCode != 200)
                    {
                        return isFileValid;
                    }
                }
                if (doctorUpdateDto.IsProfilePictureRemoved && doctorUpdateDto.ProfilePicture == null)
                {
                    if (!(doctor.ProfilePictureFilePath == DoctorResources.CommonProfilePicturePath))
                    {
                        _fileUpload.DeleteFile(doctor.ProfilePictureFilePath);
                    }
                    doctor.ProfilePictureFilePath = DoctorResources.CommonProfilePicturePath;
                    doctor.ProfilePictureFileName = DoctorResources.CommonProfilePictureName;
                }
                if (doctor == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber };
                }
                if (doctorUpdateDto.Specialization != null)
                {
                    doctor.Specialization = doctorUpdateDto.Specialization;
                }
                if (doctorUpdateDto.Gender != null)
                {
                    doctor.Gender = doctorUpdateDto.Gender;
                }
                var isDoctorUpdated = await UpdateDoctorAsync(doctorUpdateDto, doctor, fileValidationDto);
                if (isDoctorUpdated.StatusCode != 200)
                {
                    return new ResponseDto { StatusCode = isDoctorUpdated.StatusCode, Response = isDoctorUpdated.Response };
                }
                return new ResponseDto { StatusCode = isDoctorUpdated.StatusCode, Response = isDoctorUpdated.Response };
            }
            catch
            {
                return new ResponseDto
                {
                    StatusCode = 500,
                    Response = DoctorResources.DoctorUpdateFailed
                };
            }
        }

        /// <summary>
        /// Method for Update Doctor by Calling UploadFile and UpdateDoctorProfileAsync method
        /// </summary>
        /// <param name="doctorUpdateDto"></param>
        /// <param name="doctor"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UpdateDoctorAsync(DoctorUpdateDto doctorUpdateDto, Doctor doctor, FileValidationDto fileValidationDto)
        {
            try
            {
                if (doctorUpdateDto.Degreecertificate != null)
                {
                    var fileUploaded = UploadDegreeFile(doctorUpdateDto, doctor, fileValidationDto);
                    if (fileUploaded.StatusCode != 200)
                    {
                        return fileUploaded;
                    }
                    _fileUpload.DeleteFile(doctor.DoctorDegreeFilePath);
                    doctor.DoctorDegreeFileName = fileValidationDto.FirstFileName;
                    doctor.DoctorDegreeFilePath = fileValidationDto.FirstFilePath;
                }
                if (doctorUpdateDto.ProfilePicture != null && doctorUpdateDto.IsProfilePictureRemoved)
                {
                    var fileUploaded = UploadProfilePicture(doctorUpdateDto, doctor, fileValidationDto);
                    if (fileUploaded.StatusCode != 200)
                    {
                        return fileUploaded;
                    }
                    if (!(doctor.ProfilePictureFilePath == DoctorResources.CommonProfilePicturePath))
                    {
                        _fileUpload.DeleteFile(doctor.ProfilePictureFilePath);
                    }
                    doctor.ProfilePictureFileName = fileValidationDto.SecondFileName;
                    doctor.ProfilePictureFilePath = fileValidationDto.SecondFilePath;
                }
                return await UpdateDoctorAsync(doctor);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Updating Doctor
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UpdateDoctorAsync(Doctor doctor)
        {
            doctor.ModifiedBy = doctor.DoctorId;
            doctor.ModifiedDate = DateTime.Now;
            var isDoctorUpdated = await _doctorRepository.UpdateDoctorProfileAsync(doctor);
            if (!isDoctorUpdated)
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.DoctorUpdateFailed };
            }
            return new ResponseDto { StatusCode = 200, Response = DoctorResources.DoctorUpdateSuccess };
        }

        /// <summary>
        /// Method for Uploading Degree File
        /// </summary>
        /// <param name="doctorUpdateDto"></param>
        /// <param name="doctor"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadDegreeFile(DoctorUpdateDto doctorUpdateDto, Doctor doctor, FileValidationDto fileValidationDto)
        {
            fileValidationDto.FirstFileName = _fileUpload.GetFileName(doctor.Email, fileValidationDto.FirstFileExtension);
            fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
            return _fileValidations.UploadFile(fileValidationDto.FirstFilePath, doctorUpdateDto.Degreecertificate);
        }

        /// <summary>
        /// Method for Uploading Profile Picture
        /// </summary>
        /// <param name="doctorUpdateDto"></param>
        /// <param name="doctor"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadProfilePicture(DoctorUpdateDto doctorUpdateDto, Doctor doctor, FileValidationDto fileValidationDto)
        {

            fileValidationDto.SecondFileName = _fileUpload.GetFileName(doctor.Email, fileValidationDto.SecondFileExtension);
            fileValidationDto.SecondFilePath = _fileUpload.GetFilePath(fileValidationDto.SecondFileName);
            return _fileValidations.UploadFile(fileValidationDto.SecondFilePath, doctorUpdateDto.ProfilePicture);
        }
    }
}