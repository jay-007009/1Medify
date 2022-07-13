using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PharmacyServices
{
    public class GetUploadedPrescriptionsByPharmacyMobileNumber : IGetUploadedPrescriptionsByPharmacyMobileNumber
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IFileService _fileService;

        public GetUploadedPrescriptionsByPharmacyMobileNumber(IPharmacyRepository pharmacyRepository, IFileService fileService)
        {
            _pharmacyRepository = pharmacyRepository;
            _fileService = fileService;
        }

        /// <summary>
        /// Method for Getting Pharmacy's Uploaded Prescription by pharmacy Mobile Number
        /// </summary>
        /// <param name="pharmacyMobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetUploadedPrescriptionByPharmacyMobileNumberAsync(string pharmacyMobileNumber, int pageIndex)
        {
            try
            {
                if (_pharmacyRepository.IsValidMobileNo(pharmacyMobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PharmacyResources.UnregisteredPharmacyMobileNumber };
                }
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidPageIndex };
                }
                var uploadedPrescriptions = await _pharmacyRepository.PharmacyUploadedPrescriptionByPharmacyMobileNumberAsync(pharmacyMobileNumber);
                UploadedPrescriptionDto myUploadedPrescriptionsDto;
                List<UploadedPrescriptionDto> myUploadedPrescriptionsDtos = new List<UploadedPrescriptionDto>();
                foreach (var uploadedPrescription in uploadedPrescriptions)
                {
                    myUploadedPrescriptionsDto = new UploadedPrescriptionDto
                    {
                        PrescriptionId = uploadedPrescription.PrescriptionId,
                        ProfilePicture = _fileService.GetFileFromLocation(uploadedPrescription.Prescription.Patient.ProfilePictureFilePath),
                        ProfilePictureName = uploadedPrescription.Prescription.Patient.ProfilePictureFileName,
                        PatientName = uploadedPrescription.Prescription.Patient.FirstName + " " + uploadedPrescription.Prescription.Patient.LastName,
                        PrescriptionStatus = uploadedPrescription.Prescription.PrescriptionStatus == 3 ? PrescriptionStatus.Rejected.ToString()
                                                                                                       : (uploadedPrescription.Prescription.PrescriptionStatus == 1
                                                                                                       ? PrescriptionStatus.Approved.ToString()
                                                                                                       : PrescriptionStatus.Pending.ToString()),
                        ActionDateTime = (uploadedPrescription.Prescription.ModifiedDate) == null ? uploadedPrescription.Prescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm")
                                                                                                  : Convert.ToDateTime(uploadedPrescription.Prescription.ModifiedDate).ToString("yyyy-MM-ddTHH:mm"),
                        DoctorName = (uploadedPrescription.Prescription.ApprovedByDoctor != null) ? uploadedPrescription.Prescription.ApprovedByDoctor.FirstName + " " + uploadedPrescription.Prescription.ApprovedByDoctor.LastName : null,
                        IsExpired = DateTime.Now > uploadedPrescription.Prescription.ExpiryDate ? PrescriptionResource.Expired : null,
                        Diseases = _pharmacyRepository.ReadDiseaseByIds(uploadedPrescription.Diseases.Split(",").Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList()
                    };
                    myUploadedPrescriptionsDtos.Add(myUploadedPrescriptionsDto);
                }

                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = myUploadedPrescriptionsDtos.OrderByDescending(x => x.ActionDateTime).Skip(pageIndex * 10).Take(10)
                };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
