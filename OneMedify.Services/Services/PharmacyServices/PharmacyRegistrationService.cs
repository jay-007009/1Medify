using OneMedify.DTO.Files;
using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using OneMedify.DTO.User;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PharmacyServices
{
    public class PharmacyRegistrationService : IPharmacyRegistrationService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IFileUpload _fileUpload;
        private readonly IUserValidations _userValidations;
        private readonly IOneAuthorityService _oneAuthorityService;
        private readonly IFileValidations _fileValidations;
        private readonly IEmailService _emailService;

        public PharmacyRegistrationService(IPharmacyRepository pharmacyRepository, IFileUpload fileUpload,
            IUserValidations userValidations, IOneAuthorityService oneAuthorityService, IFileValidations fileValidations, IEmailService emailService)
        {
            _pharmacyRepository = pharmacyRepository;
            _fileUpload = fileUpload;
            _userValidations = userValidations;
            _oneAuthorityService = oneAuthorityService;
            _fileValidations = fileValidations;
            _emailService = emailService;
        }

        /// <summary>
        /// Async Method for Pharmacy Registration 
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <returns></returns>
        public async Task<ResponseDto> PharmacyRegistrationAsync(PharmacySignUpDto pharmacySignUpDto)
        {
            try
            {
                if (_userValidations.IsEmailAlreadyExists(pharmacySignUpDto.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.EmailExists };
                }
                if (!_userValidations.IsEmailValid(pharmacySignUpDto.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidEmailFormat };
                }
                if (_userValidations.IsMobileNumberAlreadyExists(pharmacySignUpDto.MobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.MobileNumberExists };
                }
                if (_pharmacyRepository.VerfifyCity(pharmacySignUpDto.CityId))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidCityId };
                }
                if (Convert.ToDateTime(pharmacySignUpDto.EstablishmentDate) >= DateTime.Today)
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidDate };
                }
                FileValidationDto fileValidationDto = new FileValidationDto
                {
                    FirstFileExtension = _fileUpload.GetExtensionOfDocument(pharmacySignUpDto.PharmacistDegreeCertificate),
                    SecondFileExtension = _fileUpload.GetExtensionOfDocument(pharmacySignUpDto.Certificate)
                };
                var isPharmacistFileValid = _fileValidations.ValidateFile(pharmacySignUpDto.PharmacistDegreeCertificate, fileValidationDto.FirstFileExtension);
                if (isPharmacistFileValid.StatusCode != 200)
                {
                    return isPharmacistFileValid;
                }
                var isPharmacyFileValid = _fileValidations.ValidateFile(pharmacySignUpDto.Certificate, fileValidationDto.SecondFileExtension);
                if (isPharmacyFileValid.StatusCode != 200)
                {
                    return isPharmacyFileValid;
                }
                return await UploadFileAsync(pharmacySignUpDto, fileValidationDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyRegistrationFailed };
            }
        }

        /// <summary>
        /// Method for Registering and Uploading Pharmacy files
        /// </summary>
        /// <param name="pharmacySignUpDto"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private async Task<ResponseDto> UploadFileAsync(PharmacySignUpDto pharmacySignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                var validPharmacistFile = UploadPharmacistFile(pharmacySignUpDto, fileValidationDto);
                if (validPharmacistFile.StatusCode != 200)
                {
                    return validPharmacistFile;
                }
                var validPharmacyFile = UploadPharmacyFile(pharmacySignUpDto, fileValidationDto);
                if (validPharmacyFile.StatusCode != 200)
                {
                    return validPharmacyFile;
                }

                const string emailSubject = "OneMedify Registration";
                const string emailBody = "Registered as pharmacy successfully.";
                await _emailService.SendEmailAsync(pharmacySignUpDto.Email, emailSubject, emailBody);

                return await RegisterPharmacyAsync(pharmacySignUpDto, fileValidationDto);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Uploading Pharmacist Certificate File
        /// </summary>
        /// <param name="pharmacySignUpDto"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadPharmacistFile(PharmacySignUpDto pharmacySignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.FirstFileName = _fileUpload.GetFileName(pharmacySignUpDto.Email, fileValidationDto.FirstFileExtension);
                fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
                return _fileValidations.UploadFile(fileValidationDto.FirstFilePath, pharmacySignUpDto.PharmacistDegreeCertificate);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Uploading Pharmacy Certificate File
        /// </summary>
        /// <param name="pharmacySignUpDto"></param>
        /// <param name="fileValidationDto"></param>
        /// <returns></returns>
        private ResponseDto UploadPharmacyFile(PharmacySignUpDto pharmacySignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.SecondFileName = _fileUpload.GetFileName(pharmacySignUpDto.Email, fileValidationDto.SecondFileExtension);
                fileValidationDto.SecondFilePath = _fileUpload.GetFilePath(fileValidationDto.SecondFileName);
                return _fileValidations.UploadFile(fileValidationDto.SecondFilePath, pharmacySignUpDto.Certificate);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Async Method for Calling OneAuthority and Registering of Pharmacy 
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <returns></returns>
        private async Task<ResponseDto> RegisterPharmacyAsync(PharmacySignUpDto pharmacySignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                var isUserRegistered = await UserRegisterAsync(pharmacySignUpDto);
                if (!isUserRegistered)
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.PharmacyRegistrationFailed };
                }
                return RegisterPharmacy(pharmacySignUpDto, fileValidationDto);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Async Method for Registering Pharmacy in OneAuthority
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <returns></returns>
        private async Task<bool> UserRegisterAsync(PharmacySignUpDto pharmacySignUpDto)
        {
            try
            {
                UserRegisterModel pharmacy = new UserRegisterModel
                {
                    FullName = pharmacySignUpDto.Name,
                    Email = pharmacySignUpDto.Email.ToLower(),
                    UserName = pharmacySignUpDto.Email.ToLower(),
                    PhoneNumber = pharmacySignUpDto.MobileNumber,
                    PasswordHash = pharmacySignUpDto.Password,
                    UserClient = "1Medify",
                    Roles = new List<Roles> { new Roles { Name = "Pharmacy" } },
                    Claims = new List<Claims> { new Claims { ClaimType = "allowedclients", ClaimValue = "1AuthorityApi" } }
                };
                var result = await _oneAuthorityService.RegisterUser(pharmacy);
                if (result.StatusCode != 200)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Calling Pharmacy Registration
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <param name="fileUploadDto"></param>
        /// <returns></returns>
        private ResponseDto RegisterPharmacy(PharmacySignUpDto pharmacySignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                Pharmacy pharmacy = new Pharmacy
                {
                    Email = pharmacySignUpDto.Email.ToLower(),
                    PharmacyName = pharmacySignUpDto.Name,
                    PharmacyEstablishmentDate = Convert.ToDateTime(pharmacySignUpDto.EstablishmentDate),
                    MobileNumber = pharmacySignUpDto.MobileNumber,
                    Address = pharmacySignUpDto.Address.Trim(),
                    CityId = pharmacySignUpDto.CityId,
                    PharmacyCertificateFileName = fileValidationDto.SecondFileName,
                    PharmacyCertificateFilePath = fileValidationDto.SecondFilePath,
                    PharmacistDegreeFileName = fileValidationDto.FirstFileName,
                    PharmacistDegreeFilePath = fileValidationDto.FirstFilePath,
                    ProfilePictureFileName = DoctorResources.CommonProfilePictureName,
                    ProfilePictureFilePath = DoctorResources.CommonProfilePicturePath
                };
                return PharmacyRegistration(pharmacy);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Registering Pharmacy
        /// </summary>
        /// <param name="pharmacy"></param>
        /// <param name="fileUploadDto"></param>
        /// <returns></returns>
        private ResponseDto PharmacyRegistration(Pharmacy pharmacy)
        {
            try
            {
                var isRegistrationSuccessfull = _pharmacyRepository.PharmacyRegistration(pharmacy);
                if (!isRegistrationSuccessfull)
                {
                    return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyRegistrationFailed };
                }
                return UpdatePharmacy(pharmacy);
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
        /// <param name="fileUploadDto"></param>
        /// <returns></returns>
        private ResponseDto UpdatePharmacy(Pharmacy pharmacy)
        {
            try
            {
                var isPharmacyUpdated = _pharmacyRepository.UpdatePharmacy(pharmacy);
                if (!isPharmacyUpdated)
                {
                    return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyRegistrationFailed };
                }
                return new ResponseDto { StatusCode = 200, Response = PharmacyResources.PharmacyRegistrationSuccess };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}