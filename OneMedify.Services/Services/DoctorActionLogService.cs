using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class DoctorActionLogService : IDoctorActionLogService
    {
        private readonly IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService _getPrescriptionsSentForApprovalbyPatientByDoctorMobile;
        private readonly IPrescriptionListSentForApprovalByPharmacy _prescriptionListSentForApprovalByPharmacy;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorActionLogRepository _doctorLogRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IEmailService _emailService;


        public DoctorActionLogService(IPrescriptionListSentForApprovalByPharmacy prescriptionListSentForApprovalByPharmacy, 
                                      IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService getPrescriptionsSentForApprovalbyPatientByDoctorMobile,
                                      IDoctorRepository doctorRepository,
                                      IDoctorActionLogRepository doctorActionLogRepository,
                                      IEmailService emailService,
                                      IPrescriptionRepository prescriptionRepository)
        {
            _getPrescriptionsSentForApprovalbyPatientByDoctorMobile = getPrescriptionsSentForApprovalbyPatientByDoctorMobile;
            _prescriptionListSentForApprovalByPharmacy = prescriptionListSentForApprovalByPharmacy;
            _doctorRepository = doctorRepository;
            _doctorLogRepository = doctorActionLogRepository;
            _prescriptionRepository = prescriptionRepository;
            _emailService = emailService;
            _prescriptionRepository = prescriptionRepository;
        }

        /// <summary>
        /// Method to Get List of Prescriptions Sent For Approval By Patient by Doctor's Mobile Number
        /// </summary>
        /// <param name="doctoMobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionsSentForApprovalByPatientByDoctorMobile(string doctoMobileNumber)
        {
            try
            {
                return await _getPrescriptionsSentForApprovalbyPatientByDoctorMobile.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctoMobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorActionLogResources.InternalServerError};
            }
        }


        /// <summary>
        /// Get ListOfPrescription Sent For Appproval By Pharmacy
        /// </summary>   
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionListSentForApprovalByPharmacyAsync(string doctorMobileNumber)
        {
            try
            {
                if (!Regex.IsMatch(doctorMobileNumber, @"^[0-9]{10}$"))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidMobileFormat };
                }
                return await _prescriptionListSentForApprovalByPharmacy.GetPrescriptionListSentForApprovalByPharmacy(doctorMobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }

        public async Task<ResponseDto> UpdateDoctorAction(Prescription prescription)
        {
            try
            {
                DoctorActionLog doctorAction = new DoctorActionLog
                {
                    PrescriptionId = prescription.PrescriptionId,
                    DoctorId = (Int16)prescription.ModifiedByDoctor,
                    PrescriptionStatus = prescription.PrescriptionStatus,
                    ActionTimeStamp = DateTime.Now
                };
                var statusChange = await _doctorLogRepository.Create(doctorAction);
                if (statusChange == null)
                {
                    return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.UpdateFail };
                }
                var pendingPrescription = await _doctorLogRepository.GetPendingPrescriptionById(prescription.PrescriptionId);
                foreach (var pendingStatus in pendingPrescription)
                {
                    _doctorLogRepository.Delete(pendingStatus);
                }
                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }


        public async Task<ResponseDto> GetDoctorsLoop(Prescription prescription)
        {
            try
            {
                var actionLog = _doctorLogRepository.DoctorIdCount();
                if (actionLog.Count < 10)
                {
                    await AddTenDoctorList(prescription);
                    goto response;
                }
                List<int> doctorIds = new List<int>();
                foreach (var Ids in actionLog)
                {
                    doctorIds.Add(Ids.DoctorId);
                }
                var addedLogAction = await AddDoctorActionLog(prescription, doctorIds);
                if (addedLogAction.StatusCode != 200)
                {
                    return addedLogAction;
                }
            response: return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }


        private async Task<ResponseDto> AddTenDoctorList(Prescription prescription)
        {
            try
            {
                var actionLog = _doctorLogRepository.DoctorIdCount();
                var doctor = _doctorRepository.GetTenDoctors();

                List<int> doctorCount = new List<int>();
                foreach (var Ids in actionLog)
                {
                    doctorCount.Add(Ids.DoctorId);
                }

                List<int> doctorIds = new List<int>();
                foreach (var Ids in doctor)
                {
                    doctorIds.Add(Ids.DoctorId);
                }
                var doctorList = doctorCount.Concat(doctorIds).Take(10).ToList();
                var addedDoctorId = await AddDoctorActionLog(prescription, doctorList);
                if (addedDoctorId.StatusCode != 200)
                {
                    return addedDoctorId;
                }
                return new ResponseDto { StatusCode = 200 };

            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }


        private async Task<ResponseDto> AddDoctorActionLog(Prescription prescription, List<int> doctorIds)
        {
            try
            {
                DoctorActionLog doctorAction = new DoctorActionLog
                {
                    DoctorId = doctorIds.ElementAt(0),
                    PrescriptionId = prescription.PrescriptionId,
                    PrescriptionStatus = 2,
                    ActionTimeStamp = DateTime.Now,
                    DoctorList = string.Join<int>(",", doctorIds)
                };
                var addedDoctorAction = await _doctorLogRepository.Create(doctorAction);

                var doctor = await _doctorRepository.ReadById(addedDoctorAction.DoctorId);
                if (addedDoctorAction.Prescription.PharmacyId != null)
                {
                    await _emailService.SendEmailAsync(doctor.Email, DoctorResources.PrescriptionApprovalEmail,
                                            $"Pharmacy store {addedDoctorAction.Prescription.Pharmacy.PharmacyName} has sent prescription for approval.");
                    goto response;
                }
                await _emailService.SendEmailAsync(doctor.Email, DoctorResources.PrescriptionApprovalEmail,
                                        $"Patient {addedDoctorAction.Prescription.Patient.FirstName + " " + addedDoctorAction.Prescription.Patient.LastName} has sent prescription for approval.");

            response: return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }


        public async Task<ResponseDto> ChangeDoctorIfBusy(Prescription prescription, Doctor doctor)
        {
            try
            {
                var prescriptionToAssignNextDoctor = _doctorLogRepository.GetPendingPrescription(prescription.PrescriptionId, doctor.DoctorId);

                //Remove 1st doctor and add at last position
                var newdoctorList = prescriptionToAssignNextDoctor.DoctorList.Split(",").Select(int.Parse).ToList();
                newdoctorList.Remove(prescriptionToAssignNextDoctor.DoctorId);
                newdoctorList.Add(prescriptionToAssignNextDoctor.DoctorId);
                prescriptionToAssignNextDoctor.DoctorList = String.Join(",", newdoctorList);

                //Code to insert action log
                DoctorActionLog actionLog = new DoctorActionLog
                {
                    PrescriptionId = prescriptionToAssignNextDoctor.PrescriptionId,
                    PrescriptionStatus = prescriptionToAssignNextDoctor.PrescriptionStatus,
                    DoctorId = newdoctorList[0],
                    ActionTimeStamp = DateTime.Now,
                    DoctorList = String.Join(",", newdoctorList)
                };
                var doctorActionLog = await _doctorLogRepository.Create(actionLog);

                var existedPrescription = await _prescriptionRepository.ReadById(prescription.PrescriptionId);
                var getDoctor = await _doctorRepository.ReadById(actionLog.DoctorId);
                if (prescription.PharmacyId != null)
                {
                    await _emailService.SendEmailAsync(getDoctor.Email, DoctorResources.PrescriptionApprovalEmail,
                                             $"Pharmacy store {prescriptionToAssignNextDoctor.Prescription.Pharmacy.PharmacyName} has sent prescription for approval.");
                    goto response;
                }
                await _emailService.SendEmailAsync(getDoctor.Email, DoctorResources.PrescriptionApprovalEmail,
                                        $"Patient {existedPrescription.Patient.FirstName + " " + existedPrescription.Patient.LastName} has sent prescription for approval.");

            response: _doctorLogRepository.Delete(prescriptionToAssignNextDoctor);

                return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.DoctorBusy };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }
    }
}
