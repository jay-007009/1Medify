using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PatientServices
{
    public class GetPatientProfile : IGetPatientProfile
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFileService _fileService;

        public GetPatientProfile(IPatientRepository patientRepository, IFileService fileService)
        {
            _patientRepository = patientRepository;
            _fileService = fileService;
        }

        /// <summary>
        /// Method for Getting Patient Profile Details
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientProfileByPatientMobileNumberAsync(string mobileNumber)
        {
            try
            {
                if (_patientRepository.IsValidMobileNo(mobileNumber))
                {
                    return new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber };
                }
                var patient = await _patientRepository.ReadPatientByMobileNumber(mobileNumber);
                var diseases = patient.PatientDisease.Select(x => x.Disease.DiseaseName).Distinct().ToList();
                var patientProfile = new PatientProfileDetailsDto
                {
                    ProfilePicture = _fileService.GetFileFromLocation(patient.ProfilePictureFilePath),
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    MobileNumber = patient.MobileNumber,
                    Email = patient.Email,
                    Gender = patient.Gender,
                    Diseases = diseases,
                    DateOfBirth = patient.DateOfBirth.ToString(),
                    Weight = Convert.ToDecimal(patient.Weight),
                    Address = patient.Address,
                    State = patient.City.State.StateName,
                    City = patient.City.CityName
                };
                return new ResponseDto { StatusCode = 200, Response = patientProfile };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}