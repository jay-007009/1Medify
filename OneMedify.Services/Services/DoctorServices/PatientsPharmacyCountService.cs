using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorContracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class PatientsPharmacyCountService : IPatientsPharmacyCountService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;

        public PatientsPharmacyCountService(IPatientRepository patientRepository, IDoctorRepository doctorRepository)
        {
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<ResponseDto> GetPatientPharmacyCountAsync(string mobileNumber)
        {
            try
            {
                var doctor = await _doctorRepository.ReadDoctorByMobileNumber(mobileNumber);
                if (doctor == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.ValidDoctorMobile };
                }
                PatientPharmaciesCountDto patientPharmaciesCount = new PatientPharmaciesCountDto();
                var doctorId = await _patientRepository.GetDoctorIdByDoctoMobileAsync(mobileNumber);
                patientPharmaciesCount.MyPatients = await _doctorRepository.GetPatientsCountByDoctorIdAsync(doctorId);
                patientPharmaciesCount.Pharmacies = await _doctorRepository.GetPharmaciesCount();
                return new ResponseDto { StatusCode = 200, Response = patientPharmaciesCount };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
