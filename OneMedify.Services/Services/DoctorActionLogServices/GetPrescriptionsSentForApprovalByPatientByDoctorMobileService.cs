using OneMedify.DTO.DoctorActionLog;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorActionLogServices
{
    public class GetPrescriptionsSentForApprovalByPatientByDoctorMobileService : IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService
    {
        private readonly IDoctorActionLogRepository _doctorActionLogRepository;
        private readonly IFileService _fileService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public GetPrescriptionsSentForApprovalByPatientByDoctorMobileService(IDoctorActionLogRepository doctorActionLogRepository, IFileService fileService, IDoctorRepository doctorRepository, IPrescriptionRepository prescriptionRepository)
        {
            _doctorActionLogRepository = doctorActionLogRepository;
            _fileService = fileService;
            _doctorRepository = doctorRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        /// <summary>
        /// Method to Get List of Prescriptions Sent For Approval By Patient by Doctor's Mobile Number
        /// </summary>
        /// <param name="doctoMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionsSentForApprovalByPatientByDoctorMobile(string doctorMobileNumber)
        {
            try
            {
                if (await _doctorActionLogRepository.IsDoctorMobileNumberExistsAsync(doctorMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorActionLogResources.UnregisteredDoctorMobileNumber };
                }

                var pendingPrescription = await GetPendingPrescription(doctorMobileNumber);
                var approvedAndrejectedPrescription = await GetApprovedAndRejectedPrescription(doctorMobileNumber);

                var result = pendingPrescription.Concat(approvedAndrejectedPrescription);

                return new ResponseDto { StatusCode = 200, Response = result.OrderByDescending(x => x.ActionDateTime) };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }



        /// <summary>
        /// Get all pending prescription.
        /// </summary>
        private async Task<List<PrescriptionByPatientDto>> GetPendingPrescription(string doctoMobileNumber)
        {
            try
            {
                var doctor = await _doctorRepository.ReadDoctorByMobileNumber(doctoMobileNumber);

                var prescriptions = await _doctorActionLogRepository.GetPendingPrescriptions();
                var prescriptionList = prescriptions.Where(x => x.DoctorId == doctor.DoctorId);
                List<PrescriptionByPatientDto> doctorAssignedPrescriptions = new List<PrescriptionByPatientDto>();
                foreach (var doctorprescription in prescriptionList)
                {
                    var prescription = await _prescriptionRepository.ReadById(doctorprescription.PrescriptionId);

                    var doctorPrescription = new PrescriptionByPatientDto
                    {
                        PrescriptionId = doctorprescription.PrescriptionId,
                        ProfilePicture = _fileService.GetFileFromLocation(prescription.Patient.ProfilePictureFilePath),
                        ProfilePictureName = prescription.Patient.ProfilePictureFileName,
                        PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName,
                        PrescriptionStatus = PrescriptionStatus.Pending.ToString(),
                        Gender = prescription.Patient.Gender,
                        Age = AgeCalculator.GetAge(prescription.Patient.DateOfBirth),
                        ActionDateTime = Convert.ToDateTime(doctorprescription.ActionTimeStamp).ToString("yyyy-MM-ddTHH:mm"),
                        PrescriptionType = prescription.PrescriptionType,
                        PharmacyId = (prescription.PharmacyId == null) ? 0 : (int)prescription.PharmacyId,
                        ModifiedByPharmacy = (prescription.ModifiedByPharmacy == null) ? 0 : (int)prescription.ModifiedByPharmacy,
                        ModifiedByPatient = (prescription.ModifiedByPatient == null) ? 0 : (int)prescription.ModifiedByPatient
                    };
                    doctorAssignedPrescriptions.Add(doctorPrescription);
                }               
                return doctorAssignedPrescriptions.Where(p => (p.PharmacyId == 0 && p.ModifiedByPharmacy == 0) || (p.PharmacyId != 0 && p.ModifiedByPatient != 0)).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Get all approved and rejected prescription.
        /// </summary>
        private async Task<List<PrescriptionByPatientDto>> GetApprovedAndRejectedPrescription(string doctoMobileNumber)
        {
            try
            {

                var prescriptions = await _prescriptionRepository.GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPatient(doctoMobileNumber);

                var doctorPrescriptions = prescriptions.Select(x => new PrescriptionByPatientDto
                {
                    PrescriptionId = x.PrescriptionId,
                    ProfilePicture = _fileService.GetFileFromLocation(x.Patient.ProfilePictureFilePath),
                    ProfilePictureName = x.Patient.ProfilePictureFileName,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    PrescriptionStatus = (x.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : PrescriptionStatus.Rejected.ToString(),
                    Gender = x.Patient.Gender,
                    Age = AgeCalculator.GetAge(x.Patient.DateOfBirth),
                    ActionDateTime = (x.ModifiedDate == null) ? Convert.ToDateTime(x.CreatedDate).ToString("yyyy-MM-ddTHH:mm") : Convert.ToDateTime(x.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    PrescriptionType = x.PrescriptionType
                }).ToList();

                return doctorPrescriptions;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
