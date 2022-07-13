using OneMedify.DTO.Doctor;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PrescriptionServices
{
    public class SendForApprovalService : ISendForApprovalService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IDoctorActionLogRepository _doctorActionLogRepository;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IEmailService _emailService;

        public SendForApprovalService(IPrescriptionRepository prescriptionRepository, IDoctorActionLogRepository doctorActionLogRepository,
                                      IPharmacyRepository pharmacyRepository, IEmailService emailService)
        {
            _prescriptionRepository = prescriptionRepository;
            _doctorActionLogRepository = doctorActionLogRepository;
            _pharmacyRepository = pharmacyRepository;
            _emailService = emailService;
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
                var prescription = await _prescriptionRepository.GetPrescriptionsAsync(prescriptionId);
                if (prescription == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PrescriptionNotExistById };
                }
                if (prescription.PrescriptionStatus==1 && prescription.ExpiryDate< DateTime.Now)
                {
                    if (sendForApprovalDto.PharmacyMobileNumber != null)
                    {
                        var pharmacy = await _pharmacyRepository.GetPharmacyByMobileNumberAsync(sendForApprovalDto.PharmacyMobileNumber);
                        if (pharmacy == null)
                        {
                            return new ResponseDto { StatusCode = 400, Response = PharmacyResources.UnregisteredPharmacyMobileNumber };
                        }
                        prescription.ModifiedByPharmacy = pharmacy.PharmacyId;
                        prescription.ModifiedByPatient = null;
                    }
                    else
                    {
                        prescription.ModifiedByPatient = prescription.PatientId;
                        prescription.ModifiedByPharmacy = null;
                    }
                }
                else
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.SendForApprovalEligibility };
                }
                prescription.PrescriptionStatus = 2;
                prescription.ModifiedDate = DateTime.Now;
                var sendForApproval = await SendForApprovalAsync(prescription);
                if (sendForApproval.StatusCode != 200)
                {
                    return new ResponseDto { StatusCode = sendForApproval.StatusCode, Response = sendForApproval.Response };
                }
                List<int> doctors = new List<int>();
                List<Dto> doctorsList = new List<Dto>();
                if (prescription.PrescriptionType)
                {
                    doctors.Add((int)prescription.DoctorId);
                    doctorsList = await _doctorActionLogRepository.GetDoctorListForCreatedPrescription((int)prescription.DoctorId);
                }
                if (!prescription.PrescriptionType)
                {
                    doctorsList = await _doctorActionLogRepository.GetDoctorListForUploadedPrescription();
                }
                foreach (var dto in doctorsList)
                {
                    doctors.Add(dto.DoctorId);
                }
                if (doctors.Count < 10)
                {
                    var list = await _prescriptionRepository.GetAllDoctorsAsync(10 - (doctors.Count));
                    var remainingDoctor = list.Select(x => x.DoctorId);
                    foreach (var id in remainingDoctor)
                    {
                        doctors.Add(id);
                    }
                }
                var doctorList = string.Join(",", doctors);
                var doctorActionLogCreateDto = new DoctorActionLogCreateDto
                {
                    DoctorId = doctors[0],
                    ActionTimeStamp = DateTime.Now,
                    PrescriptionStatus = prescription.PrescriptionStatus,
                    PrescriptionId = prescription.PrescriptionId,
                    DoctorList = doctorList
                };
                await Create(doctorActionLogCreateDto);
                var doctor = await _prescriptionRepository.GetDoctorNameAsync(doctorActionLogCreateDto.DoctorId);
                if (prescription.ModifiedByPharmacy != null)
                {
                    var pharmacyName = await _prescriptionRepository.GetPharmacyNameAsync((int)prescription.ModifiedByPharmacy);
                    await _emailService.SendEmailAsync(doctor.Email,
                    "Pharmacy store has sent prescription for approval.", $"{pharmacyName} has sent prescription for approval.");

                    var patientEmail = await _prescriptionRepository.GetPatientNameAsync(prescription.PatientId);

                    await _emailService.SendEmailAsync(patientEmail,
                    "Pharmacy store has sent prescription for approval to Doctor.",
                    $"{pharmacyName} has sent your prescription for approval to Dr. {doctor.FirstName} {doctor.LastName}.");
                }
                if (prescription.ModifiedByPatient != null)
                {
                    await _emailService.SendEmailAsync(doctor.Email,
                    "Patient has sent prescription for approval.", $"{prescription.Patient.FirstName} {prescription.Patient.LastName} has sent prescription for approval.");
                }
                return new ResponseDto { StatusCode = sendForApproval.StatusCode, Response = sendForApproval.Response };
            }
            catch
            {
                return new ResponseDto
                {
                    StatusCode = 500,
                    Response = PrescriptionResource.InternalServerError
                };
            }
        }

        /// <summary>
        /// Put: api/prescription/sendForApproval/{prescriptionId}
        /// Send Prescription for Approval by Prescription Id 
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDto> SendForApprovalAsync(Prescription prescription)
        {
            var isUpdated = await _prescriptionRepository.SendForApprovalAsync(prescription);
            if (!isUpdated)
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.SendForApprovalFailed };
            }
            return new ResponseDto { StatusCode = 200, Response = StatusCodeResource.SendForApprovalSuccessfully };
        }

        /// <summary>
        /// Insertion in DoctorActionLog
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<ResponseDto> Create(DoctorActionLogCreateDto doctorActionLogCreateDto)
        {
            try
            {
                Infrastructure.Entities.DoctorActionLog doctorActionLog = new Infrastructure.Entities.DoctorActionLog
                {
                    DoctorId = doctorActionLogCreateDto.DoctorId,
                    PrescriptionId = doctorActionLogCreateDto.PrescriptionId,
                    PrescriptionStatus = doctorActionLogCreateDto.PrescriptionStatus,
                    ActionTimeStamp = DateTime.Now,
                    DoctorList = doctorActionLogCreateDto.DoctorList
                };
                var doctorActionLogCreate = await _doctorActionLogRepository.Create(doctorActionLog);
                if (doctorActionLogCreate == null)
                {
                    return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
                }
                return new ResponseDto { StatusCode = 200, Response = StatusCodeResource.CreatedSuccessfully };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
    }
}