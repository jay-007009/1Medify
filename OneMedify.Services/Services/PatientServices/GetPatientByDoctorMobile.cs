using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PatientServices
{
    public class GetPatientByDoctorMobile : IGetPatientByDoctorMobile
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFileService _fileService;

        public GetPatientByDoctorMobile(IPatientRepository patientRepository, IFileService fileService)
        {
            _patientRepository = patientRepository;
            _fileService = fileService;
        }

        public async Task<ResponseDto> GetPatientsByDoctorMobileAsync(string mobileNo, int pageIndex, string patientName)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidPageIndex };
                }
                if (await _patientRepository.IsDoctorMobileNumberExistsAsync(mobileNo))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredDoctorMobileNumber };
                }

                var doctorId = await _patientRepository.GetDoctorIdByDoctoMobileAsync(mobileNo);
                var prescriptions = await _patientRepository.GetPrescriptionsByDoctorIdAsync(doctorId);
                var patients = prescriptions.Select(prescriptions => prescriptions.Patient).Distinct();
                var patientList = patients.Select(patient => new DoctorPatientsDto
                {
                    ProfilePicture = _fileService.GetFileFromLocation(patient.ProfilePictureFilePath),
                    ProfilePictureName = patient.ProfilePictureFileName,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Gender = patient.Gender,
                    Age = AgeCalculator.GetAge(patient.DateOfBirth),
                    CreatedDate = patient.Prescriptions.Select(prescription => prescription.CreatedDate.ToString("yyyy-MM-ddTHH:mm")).First(),
                    MobileNumber = patient.MobileNumber
                }).ToList();

                var filteredDoctorPatients = PatientsFilteredByName(patientList, patientName);
                return new ResponseDto { StatusCode = 200,Response = filteredDoctorPatients.Skip( pageIndex * 10).Take(10)};
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PatientResources.InternalServerResponse };
            }
        }

        private List<DoctorPatientsDto> PatientsFilteredByName(List<DoctorPatientsDto> doctorPatients, string patientName)
        {
            try
            {
                List<DoctorPatientsDto> filteredDoctorPatients = new List<DoctorPatientsDto>();
                if (patientName != null)
                {
                    foreach (var patient in doctorPatients)
                    {
                        if ((patient.FirstName + " " + patient.LastName).Contains(patientName, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredDoctorPatients.Add(patient);
                        }
                    }
                }
                else
                {
                    filteredDoctorPatients = doctorPatients;
                }
                return filteredDoctorPatients;
            }
            catch(Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
