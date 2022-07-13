using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Shared.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PatientServices
{
    public class GetUploadedPrescriptionsByPatientMobileNumber : IGetUploadedPrescriptionsByPatientMobileNumber
    {
        private readonly IPatientRepository _patientRepository;

        public GetUploadedPrescriptionsByPatientMobileNumber(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Method for Getting Patient's Uploaded Prescription by Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetUploadedPrescriptionByPatientMobileNumberAsync(string mobileNumber, int pageIndex)
        {
            try
            {
                if (_patientRepository.IsValidMobileNo(mobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidPageIndex };
                }
                var uploadedPrescriptionList = await _patientRepository.PatientsUploadedPrescriptionByPatientMobileNumberAsync(mobileNumber, pageIndex);
                var myUploadedPrescriptions = uploadedPrescriptionList.Select(uploadedPrescription => new MyUploadedPrescriptionsDto
                {
                    PrescriptionId = uploadedPrescription.PrescriptionId,
                    PrescriptionStatus = uploadedPrescription.Prescription.PrescriptionStatus == 3 ? PrescriptionStatus.Rejected.ToString()
                                                                                                   : (uploadedPrescription.Prescription.PrescriptionStatus == 1
                                                                                                   ? PrescriptionStatus.Approved.ToString()
                                                                                                   : PrescriptionStatus.Pending.ToString()),
                    ActionDateTime = uploadedPrescription.Prescription.CreatedDate.ToString(),
                    DoctorName = uploadedPrescription.Prescription.ApprovedByDoctor == null ? null : uploadedPrescription.Prescription.PrescriptionStatus == 2 ? null : (uploadedPrescription.Prescription.ApprovedByDoctor.FirstName + " " + uploadedPrescription.Prescription.ApprovedByDoctor.LastName),
                    IsExpired = DateTime.Now > uploadedPrescription.Prescription.ExpiryDate ? PrescriptionResource.Expired : null,
                    Diseases = _patientRepository.ReadDiseaseByIds(uploadedPrescription.Diseases.Split(",").Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList()
                }).ToList();
                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = myUploadedPrescriptions
                };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}