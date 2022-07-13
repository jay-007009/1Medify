using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PrescriptionServices
{
    public class GetApprovedPrescriptionService : IGetApprovedPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IFileService _fileService;
        public GetApprovedPrescriptionService(IPrescriptionRepository prescriptionRepository, IPatientRepository patientRepository,
                                              IFileService fileService)
        {
            _prescriptionRepository = prescriptionRepository;
            _patientRepository = patientRepository;
            _fileService = fileService;
        }


        /// <summary>
        /// Method to get all patient's approved and pending prescription by patient mobile number with pagination.
        /// </summary>
        public async Task<ResponseDto> GetPatientApprovedAndPendingPrescriptions(int pageIndex, string patientMobileNumber)
        {
            try
            {
                //Check patient exist or not with given mobile number.
                var patient = await _patientRepository.ReadPatientByMobileNumber(patientMobileNumber);
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile };
                }

                var prescriptions = await GetApprovedPendingCreatedPrescriptionAsync(patient.MobileNumber);

                return new ResponseDto { StatusCode = 200, Response = prescriptions.OrderByDescending(p => p.ActionDateTime).Skip(10 * pageIndex).Take(10).ToList() };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError };
            }
        }


        /// <summary>
        /// Get patient's created by doctor and uploaded by patient and pharmacy (approved and pending) prescription using patient's mobile number .
        /// </summary>
        private async Task<List<PatientPrescriptionDto>> GetApprovedPendingCreatedPrescriptionAsync(string patientMobileNumber)
        {
            try
            {
                var result = await _prescriptionRepository.GetApprovedAndPendingPrescriptionByPatientMobileNumber(patientMobileNumber);

                var prescriptions = result.Select(p => new PatientPrescriptionDto
                {
                    PatientName = p.Patient.FirstName + " " + p.Patient.LastName,
                    PatientMobileNumber = p.Patient.MobileNumber,
                    ProfilePicture = _fileService.GetFileFromLocation(p.Patient.ProfilePictureFilePath),
                    ProfilePictureName = p.Patient.ProfilePictureFileName,
                    PrescriptionId = p.PrescriptionId,
                    PrescriptionType = p.PrescriptionType,
                    ActionDateTime = (p.ModifiedDate == null) ? p.CreatedDate.ToString("yyyy-MM-ddTHH:mm") : Convert.ToDateTime(p.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                    IsExpired = (DateTime.Now > p.ExpiryDate) ? PrescriptionResource.Expired : null,
                    PrescriptionStatus = (p.PrescriptionStatus == 2) ? PrescriptionStatus.Pending.ToString() : (p.PrescriptionStatus == 1) ? PrescriptionStatus.Approved.ToString() : null,
                    Diseases = (p.PrescriptionType == true) ? p.PrescriptionMedicines.Select(m => m.Medicine.Disease.DiseaseName).Distinct().ToList()
                                                            : _patientRepository.ReadDiseaseByIds(p.PrescriptionUpload.Diseases.Split(",")
                                                                                .Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList()
                }).ToList();

                return prescriptions;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
