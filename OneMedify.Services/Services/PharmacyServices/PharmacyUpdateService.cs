using OneMedify.DTO.Files;
using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PharmacyServices
{
    public class PharmacyUpdateService : IPharmacyUpdateService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IFileUpload _fileUpload;
        private readonly IFileValidations _fileValidations;

        public PharmacyUpdateService(IPharmacyRepository pharmacyRepository, IFileUpload fileUpload, IFileValidations fileValidations)
        {
            _pharmacyRepository = pharmacyRepository;
            _fileUpload = fileUpload;
            _fileValidations = fileValidations;
        }

        /// <summary>
        /// Method for Update Pharmacy by Mobile Number
        /// </summary>    
        /// <param name="mobileNumber"></param>
        /// <param name="pharmacyUpdateDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UpdatePharmacyAsync(string mobileNumber, PharmacyUpdateDto pharmacyUpdateDto)
        {
            try
            {
                if (pharmacyUpdateDto.ProfilePicture == null && pharmacyUpdateDto.Address == null && pharmacyUpdateDto.PharmacistDegreeCertificate == null)
                {
                    return new ResponseDto { StatusCode = 200, Response = PharmacyResources.NothingToUpdate };
                }
                if (pharmacyUpdateDto.CityId != null)
                {
                    if (_pharmacyRepository.VerfifyCity((int)pharmacyUpdateDto.CityId))
                    {
                        return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidCityId };
                    }
                }
                FileValidationDto fileValidationDto = new FileValidationDto();
                var pharmacy = await _pharmacyRepository.GetPharmacyAsync(mobileNumber);
                if (pharmacyUpdateDto.PharmacistDegreeCertificate != null)
                {
                    fileValidationDto.FirstFileExtension = _fileUpload.GetExtensionOfDocument(pharmacyUpdateDto.PharmacistDegreeCertificate);
                    var isFileValid = _fileValidations.ValidateFile(pharmacyUpdateDto.PharmacistDegreeCertificate, fileValidationDto.FirstFileExtension);
                    if (isFileValid.StatusCode != 200)
                    {
                        return isFileValid;
                    }
                }
                if (pharmacyUpdateDto.IsProfilePictureRemoved && pharmacyUpdateDto.ProfilePicture != null)
                {
                    fileValidationDto.SecondFileExtension = _fileUpload.GetExtensionOfImage(pharmacyUpdateDto.ProfilePicture);
                    var isFileValid = _fileValidations.ValidateFile(pharmacyUpdateDto.ProfilePicture, fileValidationDto.SecondFileExtension);
                    if (isFileValid.StatusCode != 200)
                    {
                        return isFileValid;
                    }
                }
                if (pharmacyUpdateDto.IsProfilePictureRemoved && pharmacyUpdateDto.ProfilePicture == null)
                {
                    if (!(pharmacy.ProfilePictureFilePath == PharmacyResources.CommonProfilePicturePath))
                    {
                        _fileUpload.DeleteFile(pharmacy.ProfilePictureFilePath);
                    }
                    pharmacy.ProfilePictureFilePath = PharmacyResources.CommonProfilePicturePath;
                    pharmacy.ProfilePictureFileName = PharmacyResources.CommonProfilePictureName;
                }
                if (pharmacy == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.UnregisteredPharmacyMobileNumber };
                }
                if (pharmacyUpdateDto.ProfilePicture != null)
                {
                    pharmacy.ProfilePictureFileName = pharmacyUpdateDto.ProfilePicture;
                }
                if (pharmacyUpdateDto.Address != null)
                {
                    pharmacy.Address = pharmacyUpdateDto.Address;
                }
                if (pharmacyUpdateDto.CityId != null)
                {
                    pharmacy.CityId = pharmacyUpdateDto.CityId;
                }
                if (pharmacyUpdateDto.PharmacistDegreeCertificate != null)
                {
                    pharmacy.PharmacistDegreeFileName = pharmacyUpdateDto.PharmacistDegreeCertificate;
                }
                var isPharmacyUpdated = await UpdatePharmacyAsync(pharmacyUpdateDto, pharmacy, fileValidationDto);
                if (isPharmacyUpdated.StatusCode != 200)
                {
                    return new ResponseDto { StatusCode = isPharmacyUpdated.StatusCode, Response = isPharmacyUpdated.Response };
                }
                return new ResponseDto { StatusCode = isPharmacyUpdated.StatusCode, Response = isPharmacyUpdated.Response };
            }
            catch
            {
                return new ResponseDto
                {
                    StatusCode = 500,
                    Response = PharmacyResources.PharmacyUpdateFailed
                };
            }
        }


        /// <summary>
        /// Method for Update Pharmacy by Calling UploadFile and UpdatePharmacyProfileAsync method
        /// </summary>
        /// <param name="pharmacyUpdateDto"></param>
        /// <param name="pharmacy"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UpdatePharmacyAsync(PharmacyUpdateDto pharmacyUpdateDto, Pharmacy pharmacy, FileValidationDto fileValidationDto)
        {
            try
            {
                if (pharmacyUpdateDto.PharmacistDegreeCertificate != null)
                {
                    var fileUploaded = UploadDegreeFile(pharmacyUpdateDto, pharmacy, fileValidationDto);
                    if (fileUploaded.StatusCode != 200)
                    {
                        return fileUploaded;
                    }
                    _fileUpload.DeleteFile(pharmacy.PharmacistDegreeFilePath);
                    pharmacy.PharmacistDegreeFileName = fileValidationDto.FirstFileName;
                    pharmacy.PharmacistDegreeFilePath = fileValidationDto.FirstFilePath;
                }
                if (pharmacyUpdateDto.ProfilePicture != null && pharmacyUpdateDto.IsProfilePictureRemoved)
                {
                    var fileUploaded = UploadProfilePicture(pharmacyUpdateDto, pharmacy, fileValidationDto);
                    if (fileUploaded.StatusCode != 200)
                    {
                        return fileUploaded;
                    }
                    if (!(pharmacy.ProfilePictureFilePath == PharmacyResources.CommonProfilePicturePath))
                    {
                        _fileUpload.DeleteFile(pharmacy.ProfilePictureFilePath);
                    }
                    pharmacy.ProfilePictureFileName = fileValidationDto.SecondFileName;
                    pharmacy.ProfilePictureFilePath = fileValidationDto.SecondFilePath;
                }
                return await UpdatePharmacyAsync(pharmacy);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Method for Updating Pharmacy
        /// </summary>
        /// <param name="pharmacy"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UpdatePharmacyAsync(Pharmacy pharmacy)
        {
            pharmacy.ModifiedBy = pharmacy.PharmacyId;
            pharmacy.ModifiedDate = DateTime.Now;
            var isPharmacyUpdated = await _pharmacyRepository.UpdatePharmacyProfileAsync(pharmacy);
            if (!isPharmacyUpdated)
            {
                return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyUpdateFailed };
            }
            return new ResponseDto { StatusCode = 200, Response = PharmacyResources.PharmacyUpdateSuccess };
        }


        /// <summary>
        /// Method for Uploading Degree File
        /// </summary>
        /// <param name="pharmacyUpdateDto"></param>
        /// <param name="pharmacy"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadDegreeFile(PharmacyUpdateDto pharmacyUpdateDto, Pharmacy pharmacy, FileValidationDto fileValidationDto)
        {
            fileValidationDto.FirstFileName = _fileUpload.GetFileName(pharmacy.Email, fileValidationDto.FirstFileExtension);
            fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
            return _fileValidations.UploadFile(fileValidationDto.FirstFilePath, pharmacyUpdateDto.PharmacistDegreeCertificate);
        }


        /// <summary>
        /// Method for Uploading Profile Picture
        /// </summary>
        /// <param name="pharmacyUpdateDto"></param>
        /// <param name="pharmacy"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadProfilePicture(PharmacyUpdateDto pharmacyUpdateDto, Pharmacy pharmacy, FileValidationDto fileValidationDto)
        {

            fileValidationDto.SecondFileName = _fileUpload.GetFileName(pharmacy.Email, fileValidationDto.SecondFileExtension);
            fileValidationDto.SecondFilePath = _fileUpload.GetFilePath(fileValidationDto.SecondFileName);
            return _fileValidations.UploadFile(fileValidationDto.SecondFilePath, pharmacyUpdateDto.ProfilePicture);
        }
    }
}

