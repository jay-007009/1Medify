using OneMedify.DTO.Patient;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.DTO.User;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientDiseaseRepository _patientDiseaseRepository;
        private readonly IUserValidations _userValidations;
        private readonly IOneAuthorityService _oneAuthorityService;
        private readonly IFileService _fileService;
        private readonly IPatientUpdateService _patientUpdateService;
        private readonly IGetPatientProfile _getPatientProfile;
        private readonly IGetUploadedPrescriptionsByPatientMobileNumber _getUploadedPrescriptionsByPatientMobileNumber;
        private readonly IGetPatientsCreatedPrescriptionCount _getPatientsCreatedPrescriptionCount;
        private readonly IGetPatientByDoctorMobile _getPatientByDoctorMobile;
        private readonly IEmailService _emailService;

        public PatientService(IPatientRepository patientRepository, IPatientDiseaseRepository patientDiseaseRepository, IUserValidations userValidations,
                              IOneAuthorityService oneAuthorityService, IFileService fileService, IGetPatientProfile getPatientProfile,
                              IGetUploadedPrescriptionsByPatientMobileNumber getUploadedPrescriptionsByPatientMobileNumber, IPatientUpdateService patientUpdateService,
                              IGetPatientsCreatedPrescriptionCount getPatientsCreatedPrescriptionCount, IGetPatientByDoctorMobile getPatientByDoctorMobile,
                              IEmailService emailService)
        {
            _patientRepository = patientRepository;
            _patientDiseaseRepository = patientDiseaseRepository;
            _userValidations = userValidations;
            _oneAuthorityService = oneAuthorityService;
            _fileService = fileService;
            _getPatientProfile = getPatientProfile;
            _getUploadedPrescriptionsByPatientMobileNumber = getUploadedPrescriptionsByPatientMobileNumber;
            _patientUpdateService = patientUpdateService;
            _getPatientsCreatedPrescriptionCount = getPatientsCreatedPrescriptionCount;
            _getPatientByDoctorMobile = getPatientByDoctorMobile;
            _emailService = emailService;
        }

        /// <summary>
        /// Getting Patient Details by Patient Mobile Number 
        /// </summary>
        public async Task<ResponseDto> GetPatientByMobileNo(string mobileNo)
        {
            try
            {
                List<PatientDetailsDto> PatientList = new List<PatientDetailsDto>();

                if (_patientRepository.IsValidMobileNo(mobileNo))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }
                var patient = await _patientRepository.PatientByMobileNumberAsync(mobileNo);
                var diseases = patient.PatientDisease.Where(x => x.IsDisable == false).Select(x => x.Disease.DiseaseName).ToList();
                var patientDetails = new PatientDetailsDto
                {
                    Email = patient.Email,
                    FullName = patient.FirstName + " " + patient.LastName,
                    Age = AgeCalculator.GetAge(patient.DateOfBirth),
                    MobileNumber = patient.MobileNumber,
                    Weight = Convert.ToDecimal(patient.Weight),
                    Diseases = diseases,
                    Gender = patient.Gender
                };
                
                return new ResponseDto { StatusCode = 200, Response = patientDetails };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerIssue };
            }
        }

        /// <summary>
        /// Calling DAL Function from Infrastructure Layer and Adding response code
        /// </summary>
        public async Task<ResponseDto> PatientRegistration(PatientSignUpDto patientDto)
        {
            try
            {
                if (!(patientDto.Weight >= 0 || patientDto.Weight == null))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidWeight };
                }
                if (Convert.ToDateTime(patientDto.DateOfBirth) >= DateTime.Today)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidDate };
                }
                if (_userValidations.IsEmailAlreadyExists(patientDto.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.IsUniqueEmail };
                }
                if (!_userValidations.IsEmailValid(patientDto.Email))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.InvalidEmailFormat };
                }
                if (_userValidations.IsMobileNumberAlreadyExists(patientDto.MobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.IsUniqueMobileNo };
                }
                if (patientDto.DiseaseIds == null)
                {
                    patientDto.DiseaseIds = new List<int>();
                }
                if (patientDto.DiseaseIds.Count > 3)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidDiseaseIdCount };
                }
                List<int> patientDisease = new List<int>();
                foreach (var diseaseId in patientDto.DiseaseIds)
                {
                    if (patientDisease.Contains(diseaseId))
                    {
                        return new ResponseDto { StatusCode = 400, Response = PatientResources.MultipleDisease };
                    }
                    patientDisease.Add(diseaseId);
                }
                if (!_patientRepository.ValiadateDiseaseIds(patientDto.DiseaseIds))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidDiseaseId };
                }
                var successfullyCalled = await RegisterPatientInOneAuthorityAsync(patientDto);
                if (successfullyCalled == false)
                {
                    return new ResponseDto { StatusCode = 400, Response = OneAuthorityUserResource.SignUpFailed };
                }
                Patient registerPatient = new Patient()
                {
                    Email = patientDto.Email.ToLower(),
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    DateOfBirth = Convert.ToDateTime(patientDto.DateOfBirth),
                    MobileNumber = patientDto.MobileNumber,
                    Gender = patientDto.Gender,
                    Address = patientDto.Address.Trim(),
                    CityId = patientDto.CityId,
                    ProfilePictureFilePath = DoctorResources.CommonProfilePicturePath,
                    ProfilePictureFileName = DoctorResources.CommonProfilePictureName
                };
                if (patientDto.Weight == null)
                {
                    registerPatient.Weight = null;
                }
                else
                {
                    registerPatient.Weight = Convert.ToDouble(patientDto.Weight);
                }
                var patientAdded = _patientRepository.CreatePatient(registerPatient);
                registerPatient.CreatedBy = patientAdded.PatientId;
                var updatedPatient = _patientRepository.UpdatePatient(registerPatient);
                foreach (var diseaseId in patientDto.DiseaseIds)
                {
                    PatientDisease patientDiseases = new PatientDisease()
                    {
                        PatientId = updatedPatient.PatientId,
                        DiseaseId = diseaseId,
                        CreatedBy = updatedPatient.CreatedBy
                    };
                    _patientDiseaseRepository.CreatePatientDisease(patientDiseases);
                }

                const string emailSubject = "OneMedify Registration";
                const string emailBody = "Registered as patient successfully.";
                await _emailService.SendEmailAsync(patientDto.Email, emailSubject, emailBody);

                return new ResponseDto { StatusCode = 200, Response = PatientResources.SignUpSuccess };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerError };
            }
        }

        /// <summary>
        /// Calling 1Authority for User SignUp
        /// </summary>
        private async Task<bool> RegisterPatientInOneAuthorityAsync(PatientSignUpDto patientDto)
        {
            try
            {
                UserRegisterModel user = new UserRegisterModel()
                {
                    FullName = patientDto.FirstName + " " + patientDto.LastName,
                    Email = patientDto.Email.ToLower(),
                    UserName = patientDto.Email.ToLower(),
                    PhoneNumber = patientDto.MobileNumber,
                    PasswordHash = patientDto.Password,
                    UserClient = "1Medify",
                    Roles = new List<Roles> { new Roles { Name = "Patient" } },
                    Claims = new List<Claims> { new Claims { ClaimType = "allowedclients", ClaimValue = "1AuthorityApi" } }
                };
                var result = await _oneAuthorityService.RegisterUser(user);
                if (result.StatusCode != 200)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Async Method for fetching list of patients by Doctor's mobile number
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientsByDoctorMobileAsync(string mobileNo, int pageIndex, string patientName)
        {
            try
            {
                return await _getPatientByDoctorMobile.GetPatientsByDoctorMobileAsync(mobileNo, pageIndex, patientName);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerResponse };
            }
        }

        /// <summary>
        /// Add patient's disease
        /// </summary>
        public async Task<ResponseDto> AddPatientDisease(int patientId, int diseaseId, int doctorId)
        {
            try
            {
                //Update patient disease if it already exist with isDisable == true make it false don't add new record. 
                var patientDiseaseSoftDeleted = await _patientDiseaseRepository.ReadByIdSoftDeleted(patientId, diseaseId);
                if (patientDiseaseSoftDeleted != null)
                {
                    patientDiseaseSoftDeleted.IsDisable = false;
                    patientDiseaseSoftDeleted.ModifiedBy = doctorId;
                    patientDiseaseSoftDeleted.ModifiedDate = DateTime.Now;

                    await _patientDiseaseRepository.Update(patientDiseaseSoftDeleted);

                    return new ResponseDto { StatusCode = 200 };
                }

                PatientDisease patientDiseases = new PatientDisease()
                {
                    PatientId = patientId,
                    DiseaseId = diseaseId,
                    CreatedBy = doctorId,
                    CreatedDate = DateTime.Now,
                };
                var response = _patientDiseaseRepository.CreatePatientDisease(patientDiseases);
                if (response.PatientDiseaseId != 0)
                {
                    return new ResponseDto { StatusCode = 200 };
                }
                return new ResponseDto { StatusCode = 400 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500 };
            }
        }

        /// <summary>
        /// Update patient's disease
        /// </summary>
        public async Task<ResponseDto> RemovePatientDisease(int patientId, int diseaseId, int doctorId)
        {
            try
            {
                var patientDisease = await _patientDiseaseRepository.ReadById(patientId, diseaseId);
                if (patientDisease != null)
                {
                    patientDisease.IsDisable = true;
                    patientDisease.ModifiedBy = doctorId;
                    patientDisease.ModifiedDate = DateTime.Now;

                    await _patientDiseaseRepository.Update(patientDisease);

                    return new ResponseDto { StatusCode = 200 };
                }
                return new ResponseDto { StatusCode = 400 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 400 };
            }
        }

        /// <summary>
        /// Method: Get API for Patient's Prescriptions By Patient Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientPrescriptionByMobileNo(string mobileNumber)
        {
            try
            {
                if (_patientRepository.IsValidMobileNo(mobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }
                var prescriptions = await _patientRepository.PatientPrescriptionByPatientMobileNumberAsync(mobileNumber);

                List<PrescriptionDetailsDto> prescriptionDetailsDtos = new List<PrescriptionDetailsDto>();
                foreach (var prescription in prescriptions)
                {
                    var medicines = await _patientRepository.MedicineByPrescriptionIdAsync(prescription.PrescriptionId);
                    List<PrescriptionMedicineDetailsDto> prescriptionMedicineDtos = new List<PrescriptionMedicineDetailsDto>();
                    foreach (var medicine in medicines)
                    {
                        var prescriptionMedicine = new PrescriptionMedicineDetailsDto
                        {
                            MedicineName = medicine.Medicine.MedicineName,
                            MedicineDays = medicine.MedicineDays,
                            MedicineDosage = medicine.MedicineDosage,
                            MedicineTiming = medicine.MedicineTiming,
                            AfterBeforeMeal = medicine.AfterBeforeMeal
                        };
                        prescriptionMedicineDtos.Add(prescriptionMedicine);
                    }
                    var prescriptionmedicines = await _patientRepository.PrescriptionMedicineByPrescriptionIdAsync(prescription.PrescriptionId);
                    List<string> diseases = new List<string>();
                    foreach (var medicine in prescriptionmedicines)
                    {
                        if (!diseases.Contains(medicine.Disease.DiseaseName))
                        {
                            diseases.Add(medicine.Disease.DiseaseName);
                        }
                    }
                    var prescriptionDetailsDto = new PrescriptionDetailsDto
                    {
                        CreatedDate = prescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm"),
                        PrescriptionMedicines = prescriptionMedicineDtos,
                        Diseases = diseases
                    };
                    prescriptionDetailsDtos.Add(prescriptionDetailsDto);
                }
                return new ResponseDto { StatusCode = 200, Response = prescriptionDetailsDtos.OrderByDescending(x => x.CreatedDate) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerIssue };
            }
        }

        /// <summary>
        ///  Get List of Prescriptions Status Send For Approval by Patient Mobile Number
        /// </summary>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionStatusByPatientMobileNumberAsync(int pageIndex, string patientMobileNumber)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidPageIndex };
                }
                if (_patientRepository.IsValidMobileNo(patientMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }

                var prescriptionStatus = await _patientRepository.GetPrescriptionStatusByPatientMobileNumberAsync(patientMobileNumber);

                var prescriptionDetails = prescriptionStatus.Select(x => new PrescriptionsStatusDto
                {
                    PrescriptionId = x.PrescriptionId,
                    PrescriptionStatus = (x.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : (x.PrescriptionStatus == 2) ? PrescriptionStatus.Pending.ToString() : PrescriptionStatus.Rejected.ToString(),
                    DoctorName = x.Doctor.FirstName + " " + x.Doctor.LastName,
                    Diseases = x.PrescriptionMedicines.Select(y => y.Medicine.Disease.DiseaseName).Distinct().ToList(),
                    ActionDateTime = Convert.ToDateTime(x.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                }).ToList();
                return new ResponseDto { StatusCode = 200, Response = prescriptionDetails.OrderByDescending(x => x.ActionDateTime).Skip(pageIndex * 10).Take(10) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerIssue };
            }
        }

        /// <summary>
        /// Get List of Uploaded Prescriptions By Pharmacy by Patient Mobile Number
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UploadedPrescriptionByPharmacyPatientMobileNumberAsync(int pageIndex, string patientMobileNumber)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidPageIndex };
                }
                if (_patientRepository.IsValidMobileNo(patientMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }

                var uploadedPrescriptions = await _patientRepository.UploadedPrescriptionByPharmacyPatientMobileNumberAsync(patientMobileNumber);

                var uploadedPrescriptionsPharmacy = uploadedPrescriptions.Select(uploadedPrescription => new UploadedPrescriptionsByPharmacyDto
                {
                    PrescriptionId = uploadedPrescription.Prescription.PrescriptionId,
                    PrescriptionStatus = (uploadedPrescription.Prescription.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : (uploadedPrescription.Prescription.PrescriptionStatus == 2) ? PrescriptionStatus.Pending.ToString() : PrescriptionStatus.Rejected.ToString(),
                    PharmacyName = uploadedPrescription.Prescription.Pharmacy.PharmacyName,
                    DoctorName = (uploadedPrescription.Prescription.PrescriptionStatus == 2) ? null : (uploadedPrescription.Prescription.ApprovedByDoctor != null) ? uploadedPrescription.Prescription.ApprovedByDoctor.FirstName + " " + uploadedPrescription.Prescription.ApprovedByDoctor.LastName : null,
                    ActionDateTime = (uploadedPrescription.Prescription.ModifiedDate == null) ? uploadedPrescription.Prescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm")
                                                                                            : Convert.ToDateTime(uploadedPrescription.Prescription.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    IsExpired = (DateTime.Now > uploadedPrescription.Prescription.ExpiryDate) ? PrescriptionResource.Expired : null,
                    Diseases = _patientRepository.ReadDiseaseByIds(uploadedPrescription.Diseases.Split(",").Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList()
                }).ToList();
                return new ResponseDto { StatusCode = 200, Response = uploadedPrescriptionsPharmacy.OrderByDescending(x => x.ActionDateTime).Skip(pageIndex * 10).Take(10) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerIssue };
            }
        }

        public async Task<ResponseDto> UpdatePatientAsync(string mobileNumber, PatientUpdateDto patientUpdateDto)
        {
            try
            {
                return await _patientUpdateService.UpdatePatientAsync(mobileNumber, patientUpdateDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.PatientUpdateFailed };
            }
        }

        /// <summary>
        /// Method for Getting Patient Profile Details by Patient MobileNumber
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientProfileAsync(string mobileNumber)
        {
            try
            {
                return await _getPatientProfile.GetPatientProfileByPatientMobileNumberAsync(mobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerResponse };
            }
        }

        /// <summary>
        /// Method for Getting Patient's Uploaded Prescription 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientUploadedPrescriptionAsync(string mobileNumber, int pageIndex)
        {
            try
            {
                return await _getUploadedPrescriptionsByPatientMobileNumber.GetUploadedPrescriptionByPatientMobileNumberAsync(mobileNumber, pageIndex);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerResponse };
            }
        }

        /// <summary>
        /// Method for getting patients created prescription count by mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientsCreatedPrescriptionCountAsync(string mobileNumber)
        {
            try
            {
                return await _getPatientsCreatedPrescriptionCount.GetPatientsCreatedPrescriptionCountAsync(mobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerResponse };
            }
        }
    }
}