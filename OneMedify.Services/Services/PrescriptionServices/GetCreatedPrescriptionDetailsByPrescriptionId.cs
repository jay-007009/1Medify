using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PrescriptionServices
{
    public class GetCreatedPrescriptionDetailsByPrescriptionId : IGetCreatedPrescriptionDetailsByPrescriptionId
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public GetCreatedPrescriptionDetailsByPrescriptionId(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        public async Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
                if (prescription == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PrescriptionNotExistsById };
                }
                var medicines = prescription.PrescriptionMedicines.Select(prescriptionMedicine => prescriptionMedicine)
                                                                   .Where(prescriptionMedicine => prescriptionMedicine.IsDisable == false && prescriptionMedicine.PrescriptionId == prescriptionId)
                                                                   .ToList();
                var prescriptionDetails = new PrescriptionDetailDto
                {
                    PrescriptionId = prescription.PrescriptionId,
                    InstituteName = prescription.Doctor.InstituteName,
                    DoctorName = prescription.Doctor.FirstName + " " + prescription.Doctor.LastName,
                    InstituteAddress = prescription.Doctor.Address,
                    DoctorMobileNumber = prescription.Doctor.MobileNumber,
                    InstituteCity = prescription.Doctor.City.CityName,
                    InstituteState = prescription.Doctor.City.State.StateName,
                    CreatedDateTime = prescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm"),
                    PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName,
                    Email = prescription.Patient.Email,
                    PatientMobileNumber = prescription.Patient.MobileNumber,
                    Gender = prescription.Patient.Gender,
                    Weight = prescription.Patient.Weight,
                    Age = AgeCalculator.GetAge(prescription.Patient.DateOfBirth),
                    Diseases = prescription.PrescriptionMedicines.Where(prescriptionMedicine => prescriptionMedicine.IsDisable == false)
                                                                 .Select(prescriptionMedicine => prescriptionMedicine.Medicine.Disease.DiseaseName).Distinct().ToList(),
                    PrescriptionMedicine = medicines.Select(prescriptionMedicine => new MedicineDetailsDto
                    {
                        MedicineName = prescriptionMedicine.Medicine.MedicineName,
                        MedicineDosage = prescriptionMedicine.MedicineDosage,
                        MedicineTiming = prescriptionMedicine.MedicineTiming.Split(",").ToList(),
                        AfterBeforeMeal = prescriptionMedicine.AfterBeforeMeal,
                        MedicineDays = prescriptionMedicine.MedicineDays
                    }).ToList(),
                    PrescriptionExpiryDate = Convert.ToDateTime(prescription.ExpiryDate).ToString("yyyy-MM-ddTHH:mm"),
                    PrescriptionStatus = prescription.PrescriptionStatus == 1 ? PrescriptionStatus.Approved.ToString() : prescription.PrescriptionStatus == 2 ? PrescriptionStatus.Pending.ToString() : PrescriptionStatus.Rejected.ToString(),
                    ActionTimeStamp = prescription.PrescriptionStatus == 2 ? prescription.DoctorActionLogs.OrderByDescending(doctorActionLog => doctorActionLog.ActionTimeStamp)
                                                                                                           .Select(doctorActionLog => doctorActionLog.ActionTimeStamp)
                                                                                                           .FirstOrDefault().ToString("yyyy-MM-ddTHH:mm") : null,
                    IsExpired = prescription.ExpiryDate < DateTime.Now ? PrescriptionResource.Expired : null
                };
                return new ResponseDto { StatusCode = 200, Response = prescriptionDetails };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

    }
}