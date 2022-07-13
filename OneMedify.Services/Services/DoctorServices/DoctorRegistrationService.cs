using OneMedify.DTO.Doctor;
using OneMedify.DTO.Files;
using OneMedify.DTO.Response;
using OneMedify.DTO.User;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class DoctorRegistrationService : IDoctorRegistrationService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserValidations _userValidations;
        private readonly IFileValidations _fileValidations;
        private readonly IOneAuthorityService _oneAuthority;
        private readonly IFileUpload _fileUpload;
        private readonly IEmailService _emailService;

        public DoctorRegistrationService(IDoctorRepository doctorRepository, IUserValidations userValidations,
                                         IFileValidations fileValidations, IOneAuthorityService oneAuthority, IFileUpload fileUpload,
                                         IEmailService emailService)
        {
            _doctorRepository = doctorRepository;
            _userValidations = userValidations;
            _fileValidations = fileValidations;
            _oneAuthority = oneAuthority;
            _fileUpload = fileUpload;
            _emailService = emailService;
        }

        /// <summary>
        /// Method for Registering Doctor by Calling UploadFile and RegisterDoctorAsync method
        /// </summary>
        /// <param name="doctorSignUp"></param>
        /// <returns></returns>
        public async Task<ResponseDto> DoctorRegistrationAsync(DoctorSignUpDto doctorSignUp)
        {
            try
            {
                // Returns response if entered email id is exist in Doctor, Patient and Pharmacy Table.
                if (_userValidations.IsEmailAlreadyExists(doctorSignUp.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.EmailAlreadyExists };
                }
                if (!_userValidations.IsEmailValid(doctorSignUp.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidEmailFormat };
                }
                // Returns response if entered mobile number is exist in Doctor, Patient and Pharmacy Table.
                if (_userValidations.IsMobileNumberAlreadyExists(doctorSignUp.MobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.MobileNumberExists };
                }
                if (Convert.ToDateTime(doctorSignUp.InstituteEstablishmentDate) >= DateTime.Today)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidDate };
                }
                FileValidationDto fileValidationDto = new FileValidationDto
                {
                    FirstFileExtension = _fileUpload.GetExtensionOfDocument(doctorSignUp.DegreeCertificate),
                    SecondFileExtension = _fileUpload.GetExtensionOfDocument(doctorSignUp.InstituteCertificate)
                };
                var isDegreeFileValid = _fileValidations.ValidateFile(doctorSignUp.DegreeCertificate, fileValidationDto.FirstFileExtension);
                if (isDegreeFileValid.StatusCode != 200)
                {
                    return isDegreeFileValid;
                }
                var isInstituteFileValid = _fileValidations.ValidateFile(doctorSignUp.InstituteCertificate, fileValidationDto.SecondFileExtension);
                if (isInstituteFileValid.StatusCode != 200)
                {
                    return isInstituteFileValid;
                }
                return await UploadFileAsync(doctorSignUp, fileValidationDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.DoctorRegistrationFailed };
            }
        }

        private async Task<ResponseDto> UploadFileAsync(DoctorSignUpDto doctorSignUp, FileValidationDto fileValidationDto)
        {
            var validDegreeFile = UploadDegreeFile(doctorSignUp, fileValidationDto);
            if (validDegreeFile.StatusCode != 200)
            {
                return validDegreeFile;
            }
            var validInsituteFile = UploadInstituteFile(doctorSignUp, fileValidationDto);
            if (validInsituteFile.StatusCode != 200)
            {
                return validInsituteFile;
            }
            var doctor = await RegisterDoctorAsync(doctorSignUp, fileValidationDto);
            if (doctor)
            {
                const string emailSubject = "OneMedify Registration";
                const string emailBody = "Registered as doctor successfully.";
                await _emailService.SendEmailAsync(doctorSignUp.Email, emailSubject, emailBody);

                return new ResponseDto { StatusCode = 200, Response = DoctorResources.DoctorRegistrationSuccess };
            }
            return new ResponseDto { StatusCode = 500, Response = DoctorResources.DoctorRegistrationFailed };
        }

        /// <summary>
        /// Method for Validating and Uploading One File at a Time on Server
        /// </summary>
        /// <param name="file"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private ResponseDto UploadDegreeFile(DoctorSignUpDto doctorSignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.FirstFileName = _fileUpload.GetFileName(doctorSignUpDto.Email, fileValidationDto.FirstFileExtension);
                fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
                return _fileValidations.UploadFile(fileValidationDto.FirstFilePath, doctorSignUpDto.DegreeCertificate);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private ResponseDto UploadInstituteFile(DoctorSignUpDto doctorSignUpDto, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.SecondFileName = _fileUpload.GetFileName(doctorSignUpDto.Email, fileValidationDto.SecondFileExtension);
                fileValidationDto.SecondFilePath = _fileUpload.GetFilePath(fileValidationDto.SecondFileName);
                return _fileValidations.UploadFile(fileValidationDto.SecondFilePath, doctorSignUpDto.InstituteCertificate);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Async Method for Registering Doctor in OneAuthority
        /// </summary>
        /// <param name="doctorSignUp"></param>
        /// <returns></returns>
        private async Task<bool> OneAuthUserRegisterAsync(DoctorSignUpDto doctorSignUp)
        {
            try
            {
                UserRegisterModel user = new UserRegisterModel
                {
                    FullName = doctorSignUp.FirstName + " " + doctorSignUp.LastName,
                    Email = doctorSignUp.Email.ToLower(),
                    UserName = doctorSignUp.Email.ToLower(),
                    PhoneNumber = doctorSignUp.MobileNumber,
                    PasswordHash = doctorSignUp.Password,
                    UserClient = "1Medify",
                    Roles = new List<Roles> { new Roles { Name = "Doctor" } },
                    Claims = new List<Claims> { new Claims { ClaimType = "allowedclients", ClaimValue = "1AuthorityApi" } }
                };
                var result = await _oneAuthority.RegisterUser(user);

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
        /// Method for Registering Doctor
        /// </summary>
        /// <param name="doctorSignUp"></param>
        /// <param name="degreeFile"></param>
        /// <param name="institiuteFile"></param>
        /// <returns></returns>
        private async Task<bool> RegisterDoctorAsync(DoctorSignUpDto doctorSignUp, FileValidationDto fileValidationDto)
        {
            try
            {
                var user = await OneAuthUserRegisterAsync(doctorSignUp);
                if (user == true)
                {
                    Doctor doctor = new Doctor
                    {
                        Email = doctorSignUp.Email.ToLower(),
                        FirstName = doctorSignUp.FirstName,
                        LastName = doctorSignUp.LastName,
                        Specialization = doctorSignUp.Specialization,
                        Gender = doctorSignUp.Gender,
                        MobileNumber = doctorSignUp.MobileNumber,
                        InstituteName = doctorSignUp.InstituteName,
                        InstituteCertificateFileName = fileValidationDto.SecondFileName,
                        InstituteCertificateFilePath = fileValidationDto.SecondFilePath,
                        InstituteEstablishmentDate = Convert.ToDateTime(doctorSignUp.InstituteEstablishmentDate),
                        Address = doctorSignUp.Address.Trim(),
                        CityId = doctorSignUp.CityId,
                        DoctorDegreeFileName = fileValidationDto.FirstFileName,
                        DoctorDegreeFilePath = fileValidationDto.FirstFilePath,
                        ProfilePictureFilePath = DoctorResources.CommonProfilePicturePath,
                        ProfilePictureFileName = DoctorResources.CommonProfilePictureName
                    };
                    var newDoctor = await _doctorRepository.AddDoctorAsync(doctor);
                    if (newDoctor)
                    {
                        return await _doctorRepository.UpdateDoctorAsync(doctor);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}