using OneMedify.DTO.Files;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Shared.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PatientServices
{
    public class PatientUpdateService : IPatientUpdateService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFileUpload _fileUpload;
        private readonly IFileValidations _fileValidations;
        private readonly IPharmacyRepository _pharmacyRepository;

        public PatientUpdateService(IPatientRepository patientRepository, IFileUpload fileUpload, IFileValidations fileValidations, IPharmacyRepository pharmacyRepository)
        {
            _patientRepository = patientRepository;
            _fileUpload = fileUpload;
            _fileValidations = fileValidations;
            _pharmacyRepository = pharmacyRepository;
        }

        /// <summary>
        /// Method for Update Patient by Mobile Number
        /// </summary>  ``  
        /// <param name="mobileNumber"></param>
        /// <param name="patientUpdateDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UpdatePatientAsync(string mobileNumber, PatientUpdateDto patientUpdateDto)
        {
            try
            {
                if (patientUpdateDto.Weight == null && patientUpdateDto.Address == null && patientUpdateDto.Gender == null && patientUpdateDto.ProfilePicture == null)
                {
                    return new ResponseDto { StatusCode = 200, Response = PatientResources.NothingToUpdate };
                }
                if (patientUpdateDto.CityId != null)
                {
                    if (_pharmacyRepository.VerfifyCity((int)patientUpdateDto.CityId))
                    {
                        return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidCityId };
                    }
                }         
                if (!(patientUpdateDto.Weight >= 0 || patientUpdateDto.Weight == null))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidWeight };
                }
                FileValidationDto fileValidationDto = new FileValidationDto();
                var patient = await _patientRepository.GetPatientAsync(mobileNumber);
                if (patientUpdateDto.IsProfilePictureRemoved && patientUpdateDto.ProfilePicture != null)
                {
                    fileValidationDto.SecondFileExtension = _fileUpload.GetExtensionOfImage(patientUpdateDto.ProfilePicture);
                    var isFileValid = _fileValidations.ValidateFile(patientUpdateDto.ProfilePicture, fileValidationDto.SecondFileExtension);
                    if (isFileValid.StatusCode != 200)
                    {
                        return isFileValid;
                    }
                }
                if (patientUpdateDto.IsProfilePictureRemoved && patientUpdateDto.ProfilePicture == null)
                {
                    if (!(patient.ProfilePictureFilePath == Path.Combine(Environment.CurrentDirectory, PatientResources.CommonProfilePicturePath)))
                    {
                        _fileUpload.DeleteFile(patient.ProfilePictureFilePath);
                    }
                    patient.ProfilePictureFilePath = Path.Combine(Environment.CurrentDirectory, PatientResources.CommonProfilePicturePath);
                    patient.ProfilePictureFileName = DoctorResources.CommonProfilePictureName;
                }
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }

                if (patientUpdateDto.Gender != null)
                {
                    patient.Gender = patientUpdateDto.Gender;
                }
                if (patientUpdateDto.Weight != null)
                {
                    patient.Weight = (double?)patientUpdateDto.Weight;
                }
                if (patientUpdateDto.Address != null)
                {
                    patient.Address = patientUpdateDto.Address;
                }
                if (patientUpdateDto.CityId != null)
                {
                    patient.CityId = (int)patientUpdateDto.CityId;
                }

                var isPatientUpdated = await UpdatePatientAsync(patientUpdateDto, patient, fileValidationDto);
                if (isPatientUpdated.StatusCode != 200)
                {
                    return new ResponseDto { StatusCode = isPatientUpdated.StatusCode, Response = isPatientUpdated.Response };
                }
                return new ResponseDto { StatusCode = isPatientUpdated.StatusCode, Response = isPatientUpdated.Response };
            }
            catch
            {
                return new ResponseDto
                {
                    StatusCode = 500,
                    Response = PatientResources.PatientUpdateFailed
                };
            }
        }

        /// <summary>
        /// Method for Update Patient
        /// </summary>
        /// <param name="PatientUpdateDto"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UpdatePatientAsync(PatientUpdateDto patientUpdateDto, Patient patient, FileValidationDto fileValidationDto)
        {
            try
            {
                if (patientUpdateDto.ProfilePicture != null && patientUpdateDto.IsProfilePictureRemoved)
                {
                    var fileUploaded = UploadProfilePicture(patientUpdateDto, patient, fileValidationDto);
                    if (fileUploaded.StatusCode != 200)
                    {
                        return fileUploaded;
                    }
                    if (!(patient.ProfilePictureFilePath == Path.Combine(Environment.CurrentDirectory, PatientResources.CommonProfilePicturePath)))
                    {
                        _fileUpload.DeleteFile(patient.ProfilePictureFilePath);
                    }
                    patient.ProfilePictureFileName = fileValidationDto.SecondFileName;
                    patient.ProfilePictureFilePath = fileValidationDto.SecondFilePath;
                }
                return await UpdatePatientAsync(patient);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private async Task<ResponseDto> UpdatePatientAsync(Patient patient)
        {
            patient.ModifiedBy = patient.PatientId;
            patient.ModifiedDate = DateTime.Now;
            var isPatientUpdated = await _patientRepository.UpdatePatientProfileAsync(patient);
            if (!isPatientUpdated)
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.PatientUpdateFailed };
            }
            return new ResponseDto { StatusCode = 200, Response = PatientResources.PatientUpdateSuccess };
        }
        private ResponseDto UploadProfilePicture(PatientUpdateDto patientUpdateDto, Patient patient, FileValidationDto fileValidationDto)
        {

            fileValidationDto.SecondFileName = _fileUpload.GetFileName(patient.Email, fileValidationDto.SecondFileExtension);
            fileValidationDto.SecondFilePath = _fileUpload.GetFilePath(fileValidationDto.SecondFileName);
            return _fileValidations.UploadFile(fileValidationDto.SecondFilePath, patientUpdateDto.ProfilePicture);
        }

    }
}
