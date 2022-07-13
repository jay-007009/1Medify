using OneMedify.DTO.Doctor;
using OneMedify.DTO.Files;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IGetApprovedPrescriptionService _getPatientApprovedPrescriptionService;
        private readonly ICreatePrescriptionService _createPrescriptionService;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IDiseaseRepository _diseaseRepository;
        private readonly IUploadPrescriptionRepository _uploadPrescriptionRepository;
        private readonly IFileUpload _fileUpload;
        private readonly IFileValidations _fileValidations;
        private readonly IDoctorActionLogService _doctorActionLogService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserValidations _userValidations;
        private readonly IFileService _fileService;
        private readonly IGetCreatedPrescriptionDetailsByPrescriptionId _getCreatedPrescriptionDetailsByPrescriptionId;
        private readonly ISendForApprovalService _sendForApprovelService;
        private readonly IEmailService _emailService;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IPatientRepository patientRepository, IGetApprovedPrescriptionService getPatientApprovedPrescriptionService,
                                   ICreatePrescriptionService createPrescriptionService, IPharmacyRepository pharmacyRepository, IDiseaseRepository diseaseRepository,
                                   IUploadPrescriptionRepository uploadPrescriptionRepository, IFileUpload fileUpload, IFileValidations fileValidations,
                                   IDoctorActionLogService doctorActionLogService, IDoctorRepository doctorRepository, IUserValidations userValidations,
                                   IFileService fileService, IGetCreatedPrescriptionDetailsByPrescriptionId getCreatedPrescriptionDetailsByPrescriptionId,
                                   ISendForApprovalService sendForApprovelService, IEmailService emailService)
        {
            _prescriptionRepository = prescriptionRepository;
            _patientRepository = patientRepository;
            _getPatientApprovedPrescriptionService = getPatientApprovedPrescriptionService;
            _createPrescriptionService = createPrescriptionService;
            _pharmacyRepository = pharmacyRepository;
            _diseaseRepository = diseaseRepository;
            _uploadPrescriptionRepository = uploadPrescriptionRepository;
            _fileUpload = fileUpload;
            _fileValidations = fileValidations;
            _doctorActionLogService = doctorActionLogService;
            _doctorRepository = doctorRepository;
            _userValidations = userValidations;
            _fileService = fileService;
            _getCreatedPrescriptionDetailsByPrescriptionId = getCreatedPrescriptionDetailsByPrescriptionId;
            _sendForApprovelService = sendForApprovelService;
            _emailService = emailService;
        }

        /// <summary>
        /// Method to make call to create prescription service.
        /// </summary>
        public async Task<ResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto prescription)
        {
            try
            {
                return await _createPrescriptionService.CreatePrescription(prescription);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed };
            }
        }

        /// <summary>
        /// Method to get all patient prescription by pharmacyMobileNumber
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pharmacyMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetAllPatientPrescriptionByPharmacyMobileNumberAsync(int pageindex, string pharmacyMobileNumber)
        {
            try
            {
                if (_userValidations.IsMobileNumberValid(pharmacyMobileNumber))
                {
                    return new ResponseDto { StatusCode = 404, Response = PharmacyResources.UnregisteredPharmacyMobileNumber };
                }
                var prescription = await _prescriptionRepository.GetAllPatientPrescriptionByPharmacyMobileNumber(pharmacyMobileNumber);

                var prescriptionList = prescription.Select(prescriptions =>
                new UploadedPrescriptionDto
                {
                    PrescriptionId = prescriptions.PrescriptionId,
                    ProfilePicture = _fileService.GetFileFromLocation(prescriptions.Patient.ProfilePictureFilePath),
                    ProfilePictureName = prescriptions.Patient.ProfilePictureFileName,
                    PatientName = prescriptions.Patient.FirstName + " " + prescriptions.Patient.LastName,
                    PrescriptionStatus = (prescriptions.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : (prescriptions.PrescriptionStatus == 2) ? PrescriptionStatus.Pending.ToString() : PrescriptionStatus.Rejected.ToString(),
                    Diseases = (prescriptions.PrescriptionType == true) ? prescriptions.PrescriptionMedicines.Select(m => m.Medicine.Disease.DiseaseName).Distinct().ToList()
                               : _patientRepository.ReadDiseaseByIds(prescriptions.PrescriptionUpload.Diseases.Split(",").Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList(),
                    DoctorName = (prescriptions.PrescriptionStatus == 2) ? null : prescriptions.ApprovedByDoctor.FirstName + " " + prescriptions.ApprovedByDoctor.LastName,
                    ActionDateTime = Convert.ToDateTime(prescriptions.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    IsExpired = (DateTime.Now > prescriptions.ExpiryDate) ? PrescriptionResource.Expired : null,
                    PrescriptionType = prescriptions.PrescriptionType
                }).ToList();
                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = prescriptionList.OrderByDescending(x => x.ActionDateTime).Skip(pageindex * 10).Take(10)
                };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }


        /// <summary>
        /// Method to Get Prescription By PrescriptionId
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {

            try
            {
                if (_prescriptionRepository.IsUploadedPrescriptionIdExist(prescriptionId))
                {
                    return new ResponseDto { StatusCode = 404, Response = PrescriptionResource.PrescriptionNotExistById };
                }
                var prescriptionUpload = await _prescriptionRepository.GetPrescriptionByPrescriptionIdAsync(prescriptionId);
                PrescriptionUploadDto prescriptionUploadDto = new PrescriptionUploadDto
                {
                    PrescriptionId = prescriptionUpload.PrescriptionId,
                    Diseases = _patientRepository.ReadDiseaseByIds(prescriptionUpload.Diseases.Split(",").Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList(),
                    PrescriptionStatus = (prescriptionUpload.Prescription.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString()
                                                                                             : (prescriptionUpload.Prescription.PrescriptionStatus == 2)
                                                                                             ? PrescriptionStatus.Pending.ToString()
                                                                                             : PrescriptionStatus.Rejected.ToString(),
                    PrescriptionFile = _fileService.GetFileFromLocation(prescriptionUpload.PrescriptionFilePath),
                    PrescriptionFileName = prescriptionUpload.PrescriptionFileName,
                    PrescriptionFilePath = prescriptionUpload.PrescriptionFilePath,
                    PrescriptionExpiryDate = (prescriptionUpload.Prescription.ExpiryDate == null) ? null : Convert.ToDateTime(prescriptionUpload.Prescription.ExpiryDate).ToString("yyyy-MM-ddTHH:mm"),
                    PrescriptionType = prescriptionUpload.Prescription.PrescriptionType,
                    IsExpired = prescriptionUpload.Prescription.ExpiryDate < DateTime.Now ? PrescriptionResource.Expired : null,
                    ActionTimeStamp = prescriptionUpload.Prescription.DoctorActionLogs.Count == 0 ? null : prescriptionUpload.Prescription.PrescriptionStatus == 2 ? prescriptionUpload.Prescription.DoctorActionLogs.OrderByDescending(doctorActionLog => doctorActionLog.ActionTimeStamp)
                                                                                                           .Select(doctorActionLog => doctorActionLog.ActionTimeStamp)
                                                                                                           .FirstOrDefault().ToString("yyyy-MM-ddTHH:mm") : null,

                };
                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = prescriptionUploadDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }


        /// <summary>
        /// Method to get all patient's approved and pending prescription by patient mobile number.
        /// </summary>
        public async Task<ResponseDto> GetApprovedAndPendingPrescriptionsAsync(int pageIndex, string patientMobileNumber)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex };
                }

                //Validate mobile number format.
                if (!Regex.IsMatch(patientMobileNumber, @"^[0-9]{10}$"))
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidMobileFormat };
                }

                return await _getPatientApprovedPrescriptionService.GetPatientApprovedAndPendingPrescriptions(pageIndex, patientMobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }



        /// <summary>
        /// Method to get all patient's created prescription by doctor which are sent for approval by pharmacy.
        /// </summary>
        public async Task<ResponseDto> GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(int pageIndex, string patientMobileNumber)
        {
            try
            {
                //Check patient exist or not with given mobile number.
                var patient = await _patientRepository.ReadPatientByMobileNumber(patientMobileNumber);
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile };
                }

                var result = await _prescriptionRepository.GetCreatedPrescriptionSentForApprovalByPharmacy(patientMobileNumber);
                var prescriptions = result.Select(p => new CreatedPrescriptionsByPharmacyDto
                {
                    PrescriptionId = p.PrescriptionId,
                    PrescriptionStatus = (p.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : (p.PrescriptionStatus == 2)
                                                                     ? PrescriptionStatus.Pending.ToString() : PrescriptionStatus.Rejected.ToString(),
                    ActionDateTime = Convert.ToDateTime(p.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    PharmacyName = p.SentFromPharmacy.PharmacyName,
                    DoctorName = (p.PrescriptionStatus != 2) ? p.Doctor.FirstName + " " + p.Doctor.LastName : null,
                    IsExpired = (DateTime.Now > p.ExpiryDate) ? PrescriptionResource.Expired : null,
                    Diseases = p.PrescriptionMedicines.Select(pm => pm.Medicine.Disease.DiseaseName).Distinct().ToList()
                }).ToList();

                return new ResponseDto { StatusCode = 200, Response = prescriptions.OrderByDescending(p => p.ActionDateTime).Skip(10 * pageIndex).Take(10).ToList() };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }

        public async Task<ResponseDto> UpdatePrescriptionStatus(DoctorActionDto doctorActionDto, int prescriptionId)
        {
            try
            {
                var existedPrescription = await _prescriptionRepository.ReadById(prescriptionId);
                if (existedPrescription == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PrescriptionNotExistsById };
                }
                if (existedPrescription.PrescriptionStatus != 2)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.ActionCanNotBeTaken };
                }
                var doctor = await _doctorRepository.GetDoctorAsync(doctorActionDto.DoctorMobileNumber);
                if (doctor == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DoctorNotExistByMobile };
                }
                if (doctorActionDto.Action == DoctorAction.Busy.ToString())
                {
                    return await _doctorActionLogService.ChangeDoctorIfBusy(existedPrescription, doctor);
                }
                if(doctorActionDto.ExpiryDate == "null")
                {
                    existedPrescription.ExpiryDate = null;
                }
                else
                {
                    existedPrescription.ExpiryDate = Convert.ToDateTime(doctorActionDto.ExpiryDate + " " + DateTime.Now.ToString("HH:mm:ss"));
                }                
                existedPrescription.PrescriptionStatus = (doctorActionDto.Action == DoctorAction.Approve.ToString()) ? 1 : 3;
                existedPrescription.ModifiedByDoctor = doctor.DoctorId;
                existedPrescription.ModifiedDate = DateTime.Now;

                var updatePrescription = _prescriptionRepository.UpdatePrescription(existedPrescription);
                if (updatePrescription == null)
                {
                    return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.UpdateFail };
                }
                var statusUpdated = await _doctorActionLogService.UpdateDoctorAction(updatePrescription);
                if (statusUpdated.StatusCode != 200)
                {
                    return statusUpdated;
                }

                if (doctorActionDto.Action == DoctorAction.Reject.ToString())
                {
                    //Send email notification to patient that prescription is rejected.
                    await _emailService.SendEmailAsync(existedPrescription.Patient.Email, PrescriptionResource.Reject, PrescriptionResource.RejectEmail);
                    return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.PrescriptionRejected };
                }

                //Send email notification to patient that prescription is approved.
                await _emailService.SendEmailAsync(existedPrescription.Patient.Email, PrescriptionResource.Approve, PrescriptionResource.ApproveEmail);
                return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.PrescriptionApproved };
            }
            catch(Exception e)
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }


        /// <summary>
        /// Get All PrescriptionCount By DoctorMobileNumber For Last 30 Days
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionsCountAsync(string doctorMobileNumber)
        {
            try
            {
                if (_prescriptionRepository.IsMobileNumberValid(doctorMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber };
                }
                var prescriptionCount = _prescriptionRepository.GetPrescriptionsCountAsync(doctorMobileNumber).Result;

                var countOfprescription = prescriptionCount.GroupBy(x => x.CreatedDate.ToString("yyyy-MM-dd"))
                .Select(x => new PrescriptionsCountDto
                {
                    CreatedDate = x.Key,
                    PrescriptionCount = x.Count()
                }).ToList();

                var dateTime = Enumerable.Range(0, 1 + DateTime.Now.Subtract(DateTime.Now.AddDays(-30)).Days)
                              .Select(offset => DateTime.Now.AddDays(-offset).Date).ToList();
                var lastThirtyDate = dateTime.Select(date => date.ToString("yyyy-MM-dd"));

                var prescriptionDate = countOfprescription.Select(prescription => prescription.CreatedDate).ToList();

                var notPrescribedDates = lastThirtyDate.Except(prescriptionDate).ToList();

                foreach (var notPrescribedDate in notPrescribedDates)
                {
                    countOfprescription.Add(new PrescriptionsCountDto { CreatedDate = notPrescribedDate, PrescriptionCount = 0 });
                }

                return new ResponseDto { StatusCode = 200, Response = countOfprescription.OrderBy(x => x.CreatedDate) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }



        /// <summary>
        /// Service to Upload Prescription By Pharmacy
        /// </summary>
        /// <param name="uploadPatientPrescriptionDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UploadPrescriptionByPharmacy(UploadPatientPrescriptionDto uploadPatientPrescriptionDto)
        {
            try
            {
                var pharmacy = await _pharmacyRepository.GetPharmacyByMobileNumberAsync(uploadPatientPrescriptionDto.PharmacyMobileNumber);
                if (pharmacy == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PharmacyNotExistByMobile };
                }

                //Check patient with given mobile number exist or not. and get patient details.
                var patient = await _patientRepository.ReadPatientByMobileNumber(uploadPatientPrescriptionDto.PatientMobileNumber);
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile };
                }

                foreach (var diseaseId in uploadPatientPrescriptionDto.DiseaseIds)
                {
                    //Check given disease with disease id exist or not.
                    var existingDisease = _diseaseRepository.ReadById(diseaseId);
                    if (existingDisease == null)
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DiseaseNotExistById };
                    }
                }
                FileValidationDto fileValidationDto = new FileValidationDto
                {
                    FirstFileExtension = _fileUpload.GetExtensionOfDocument(uploadPatientPrescriptionDto.PrescriptionDocument)
                };
                var isFileValid = _fileValidations.ValidateFile(uploadPatientPrescriptionDto.PrescriptionDocument, fileValidationDto.FirstFileExtension);
                if (isFileValid.StatusCode != 200)
                {
                    return isFileValid;
                }

                Prescription prescription = new Prescription
                {
                    PharmacyId = pharmacy.PharmacyId,
                    PatientId = patient.PatientId,
                    CreatedBy = (int)pharmacy.PharmacyId,
                    PrescriptionStatus = 2,
                    CreatedDate = DateTime.Now,
                    PrescriptionType = false
                };
                var uploadPrescriptionByPharmacy = await _prescriptionRepository.Create(prescription);
                var uploadPrescriptionResponse = UploadPrescription(uploadPatientPrescriptionDto, uploadPrescriptionByPharmacy, fileValidationDto);
                if (uploadPrescriptionResponse.StatusCode != 200)
                {
                    return uploadPrescriptionResponse;
                }
                var action = await _doctorActionLogService.GetDoctorsLoop(prescription);
                if (action.StatusCode != 200)
                {
                    return action;
                }
                return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.UploadedSuccessfully };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.UploadFailed };
            }
        }



        /// <summary>
        /// Method to Add Data in PrescriptionUpload Table
        /// </summary>
        /// <param name="uploadPatientPrescriptionDto"></param>
        /// <param name="prescription"></param>
        /// <returns></returns>
        private ResponseDto UploadPrescription(UploadPatientPrescriptionDto uploadPatientPrescriptionDto, Prescription prescription, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.FirstFileName = _fileUpload.GetFileName(prescription.Pharmacy.Email, fileValidationDto.FirstFileExtension);
                fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
                _fileValidations.UploadFile(fileValidationDto.FirstFilePath, uploadPatientPrescriptionDto.PrescriptionDocument);

                List<int> diseases = new List<int>();
                foreach (var diseaseId in uploadPatientPrescriptionDto.DiseaseIds)
                {
                    diseases.Add(diseaseId);
                }

                if (prescription.PrescriptionId != 0)
                {
                    PrescriptionUpload prescriptionUpload = new PrescriptionUpload
                    {
                        PrescriptionId = prescription.PrescriptionId,
                        PrescriptionFileName = fileValidationDto.FirstFileName,
                        PrescriptionFilePath = fileValidationDto.FirstFilePath,
                        Diseases = string.Join<int>(",", diseases),
                        IsDisable = false
                    };
                    _uploadPrescriptionRepository.Create(prescriptionUpload);
                }
                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.UploadFailed };
            }
        }

        public async Task<ResponseDto> UploadPrescriptionByPatient(UploadPrescriptionDto uploadPrescriptionDto)
        {
            try
            {
                //Check patient with given mobile number exist or not. and get patient details.
                var patient = await _patientRepository.ReadPatientByMobileNumber(uploadPrescriptionDto.PatientMobileNumber);
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile };
                }

                foreach (var diseaseId in uploadPrescriptionDto.DiseaseIds)
                {
                    //Check given disease with disease id exist or not.
                    var existingDisease = _diseaseRepository.ReadById(diseaseId);
                    if (existingDisease == null)
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DiseaseNotExistById };
                    }
                }

                FileValidationDto fileValidationDto = new FileValidationDto
                {
                    FirstFileExtension = _fileUpload.GetExtensionOfDocument(uploadPrescriptionDto.PrescriptionDocument)
                };
                var isFileValid = _fileValidations.ValidateFile(uploadPrescriptionDto.PrescriptionDocument, fileValidationDto.FirstFileExtension);
                if (isFileValid.StatusCode != 200)
                {
                    return isFileValid;
                }

                Prescription prescription = new Prescription
                {
                    PatientId = patient.PatientId,
                    CreatedBy = patient.PatientId,
                    PrescriptionStatus = 2,
                    CreatedDate = DateTime.Now,
                    PrescriptionType = false,
                };
                var uploadPrescription = await _prescriptionRepository.Create(prescription);
                var uploadPrescriptionResponse = UploadPrescription(uploadPrescriptionDto, uploadPrescription, fileValidationDto);
                if (uploadPrescriptionResponse.StatusCode != 200)
                {
                    return uploadPrescriptionResponse;
                }
                var action = await _doctorActionLogService.GetDoctorsLoop(prescription);
                if (action.StatusCode != 200)
                {
                    return action;
                }
                return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.UploadedSuccessfully };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.UploadFailed };
            }
        }

        private ResponseDto UploadPrescription(UploadPrescriptionDto uploadPrescriptionDto, Prescription prescription, FileValidationDto fileValidationDto)
        {
            try
            {
                fileValidationDto.FirstFileName = _fileUpload.GetFileName(prescription.Patient.Email, fileValidationDto.FirstFileExtension);
                fileValidationDto.FirstFilePath = _fileUpload.GetFilePath(fileValidationDto.FirstFileName);
                _fileValidations.UploadFile(fileValidationDto.FirstFilePath, uploadPrescriptionDto.PrescriptionDocument);

                List<int> diseases = new List<int>();
                foreach (var diseaseId in uploadPrescriptionDto.DiseaseIds)
                {
                    diseases.Add(diseaseId);
                }

                if (prescription.PrescriptionId != 0)
                {
                    PrescriptionUpload prescriptionUpload = new PrescriptionUpload
                    {
                        PrescriptionId = prescription.PrescriptionId,
                        PrescriptionFileName = fileValidationDto.FirstFileName,
                        PrescriptionFilePath = fileValidationDto.FirstFilePath,
                        Diseases = string.Join<int>(",", diseases),
                        IsDisable = false
                    };
                    _uploadPrescriptionRepository.Create(prescriptionUpload);
                }
                return new ResponseDto { StatusCode = 200 };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        ///  Get Created Prescription by Prescription Id
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            try
            {
                if (_prescriptionRepository.IsCreatedPrescriptionIdExist(prescriptionId))
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PrescriptionNotExistById };
                }
                return await _getCreatedPrescriptionDetailsByPrescriptionId.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }

        /// <summary>
        /// Put: api/prescription/sendForApproval/{prescriptionId}
        /// Send Prescription for Approval by Prescription Id 
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<ResponseDto> SendForApprovalAsync(int prescriptionId, SendForApprovalDto sendForApprovalDto)
        {
            try
            {
                return await _sendForApprovelService.SendForApprovalAsync(prescriptionId, sendForApprovalDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.DoctorUpdateFailed };
            }
        }

        /// <summary>
        /// GET: api/patient/createdPrescriptions/{patientMobileNumber}
        /// API to get all approved created prescription by  patient mobile number.
        /// </summary>

        public async Task<ResponseDto> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(int pageIndex, string patientMobileNumber)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex };
                }

                List<ApprovedCreatedPrescriptionsDto> approvedCreatedPrescriptionsList = new List<ApprovedCreatedPrescriptionsDto>();

                if (_prescriptionRepository.IsValidPatientMobileNumber(patientMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.MobileNumberIsValid };
                }
                if (_prescriptionRepository.MobileNumbereExists(patientMobileNumber))
                {
                    return new ResponseDto { StatusCode = 404, Response = PrescriptionResource.MobileNumberExists };
                }
                var approvedCreatedPrescriptions = await _prescriptionRepository.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(pageIndex, patientMobileNumber);

                foreach (var approvedCreatedPrescription in approvedCreatedPrescriptions)
                {
                    ApprovedCreatedPrescriptionsDto approvedCreatedPrescriptionDto = new ApprovedCreatedPrescriptionsDto()
                    {
                        PrescriptionId = approvedCreatedPrescription.PrescriptionId,
                        Diseases = approvedCreatedPrescription.PrescriptionMedicines.Select(x => x.Medicine.Disease.DiseaseName).ToList(),
                        DoctorName = approvedCreatedPrescription.Doctor.FirstName + " " + approvedCreatedPrescription.Doctor.LastName,
                        IsExpired = (DateTime.Now > approvedCreatedPrescription.ExpiryDate) ? PrescriptionResource.Expired : null,
                        ActionDateTime = (approvedCreatedPrescription.ModifiedDate == null) ? approvedCreatedPrescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm")
                                                                                            : Convert.ToDateTime(approvedCreatedPrescription.ModifiedDate).ToString("yyyy-MM-ddTHH:mm")
                    };

                    approvedCreatedPrescriptionsList.Add(approvedCreatedPrescriptionDto);
                }

                if (Math.Ceiling((decimal)approvedCreatedPrescriptionsList.Count / 10) - 1 < pageIndex)
                {
                    return new ResponseDto { StatusCode = 200, Response = new List<ApprovedCreatedPrescriptionsDto>() };
                }

                return new ResponseDto { StatusCode = 200, Response = approvedCreatedPrescriptionsList.GetRange(pageIndex * 10, approvedCreatedPrescriptionsList.Count < (pageIndex * 10) + 10 ? approvedCreatedPrescriptionsList.Count % 10 : 10) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerIssue };
            }
        }
    }
}