using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PatientContracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PatientServices
{
    public class GetPatientsCreatedPrescriptionCount : IGetPatientsCreatedPrescriptionCount
    {
        private readonly IPatientRepository _patientRepository;

        public GetPatientsCreatedPrescriptionCount(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Method for getting patients created prescription count by mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientsCreatedPrescriptionCountAsync(string mobileNumber)
        {
            try
            {
                if (_patientRepository.IsValidMobileNo(mobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }
                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = new CreatedPrescriptionsCountDto
                    {
                        CreatedPrescriptionsCount = await _patientRepository.PatientCreatedPrescriptionByPatientMobileNumberAsync(mobileNumber)
                    }
                };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}