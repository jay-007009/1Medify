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
    public class PrescriptionListSentForApprovalByPharmacy : IPrescriptionListSentForApprovalByPharmacy
    {
        private readonly IDoctorActionLogRepository _doctorActionLogRepository;
        private readonly IFileService _fileService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionListSentForApprovalByPharmacy(IDoctorActionLogRepository doctorActionLogRepository,
                                                         IFileService fileService,
                                                         IDoctorRepository doctorRepository,
                                                         IPrescriptionRepository prescriptionRepository)
        {
            _doctorActionLogRepository = doctorActionLogRepository;
            _fileService = fileService;
            _doctorRepository = doctorRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        /// <summary>
        /// Get PrescriptionList SentForApproval By Pharmacy
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPrescriptionListSentForApprovalByPharmacy(string doctorMobileNumber)
        {
            try
            {
                if (_doctorRepository.IsValidMobileNumber(doctorMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber };
                }

                var pendingPrescription = await GetPendingPrescription(doctorMobileNumber);
                var approvedAndrejectedPrescription = await GetApprovedAndRejectedPrescription(doctorMobileNumber);

                var result = pendingPrescription.Concat(approvedAndrejectedPrescription);

                return new ResponseDto { StatusCode = 200, Response = result.OrderByDescending(x => x.ActionDateTime) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }


        /// <summary>
        /// Get all pending prescription.
        /// </summary>
        private async Task<List<PrescriptionByPharmacyDto>> GetPendingPrescription(string doctoMobileNumber)
        {
            try
            {
                var doctor = await _doctorRepository.ReadDoctorByMobileNumber(doctoMobileNumber);

                var prescriptions = await _doctorActionLogRepository.GetPendingPrescriptions();
                var prescriptionList = prescriptions.Where(x => x.DoctorId == doctor.DoctorId);
                List<PrescriptionByPharmacyDto> doctorAssignedPrescriptions = new List<PrescriptionByPharmacyDto>();
                foreach (var doctorprescription in prescriptionList)
                {
                    var prescription = await _prescriptionRepository.ReadById(doctorprescription.PrescriptionId);

                    var doctorPrescription = new PrescriptionByPharmacyDto
                    {
                        PrescriptionId = doctorprescription.PrescriptionId,
                        ProfilePicture = _fileService.GetFileFromLocation(prescription.Patient.ProfilePictureFilePath),
                        ProfilePictureName = prescription.Patient.ProfilePictureFileName,
                        PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName,
                        Gender = prescription.Patient.Gender,
                        PrescriptionStatus = PrescriptionStatus.Pending.ToString(),
                        PharmacyName = (prescription.ModifiedByPharmacy != null) ? prescription.SentFromPharmacy?.PharmacyName : prescription.Pharmacy?.PharmacyName,
                        ActionDateTime = doctorprescription.ActionTimeStamp.ToString("yyyy-MM-ddTHH:mm"),
                        Age = AgeCalculator.GetAge(prescription.Patient.DateOfBirth),
                        PrescriptionType = prescription.PrescriptionType,
                        PharmacyId = ((prescription.PharmacyId == null) ? 0 : (int)prescription.PharmacyId),
                        ModifiedByPharmacy = ((prescription.ModifiedByPharmacy == null) ? 0 : (int)prescription.ModifiedByPharmacy),
                    };
                    doctorAssignedPrescriptions.Add(doctorPrescription);
                }
                return doctorAssignedPrescriptions.Where(p => (p.PharmacyId != 0 || p.ModifiedByPharmacy != 0)).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Get all approved and rejected prescription.
        /// </summary>
        private async Task<List<PrescriptionByPharmacyDto>> GetApprovedAndRejectedPrescription(string doctoMobileNumber)
        {
            try
            {

                var prescriptions = await _prescriptionRepository.GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPharmacy(doctoMobileNumber);

                var doctorPrescriptions = prescriptions.Select(x => new PrescriptionByPharmacyDto
                {
                    PrescriptionId = x.PrescriptionId,
                    ProfilePicture = _fileService.GetFileFromLocation(x.Patient.ProfilePictureFilePath),
                    ProfilePictureName = x.Patient.ProfilePictureFileName,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    Gender = x.Patient.Gender,
                    PrescriptionStatus = (x.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : PrescriptionStatus.Rejected.ToString(),
                    PharmacyName = (x.ModifiedByPharmacy != null) ? x.SentFromPharmacy.PharmacyName : x.Pharmacy.PharmacyName,
                    ActionDateTime =(x.ModifiedDate == null) ? x.CreatedDate.ToString("yyyy-MM-ddTHH:mm") 
                                                            : Convert.ToDateTime(x.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    Age = AgeCalculator.GetAge(x.Patient.DateOfBirth),
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
