using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.DoctorContracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class GetDoctorsCountService : IGetDoctorsCountService
    {
        private readonly IDoctorRepository _doctorRepository;

        public GetDoctorsCountService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        /// <summary>
        /// Method To Get Count Of All Registered Doctors
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDto> GetDoctorsCountAsync()
        {
            try
            {
                DoctorsCountDto doctor = new DoctorsCountDto();
                doctor.DoctorCount = await _doctorRepository.GetDoctorsCountAsync();
                return new ResponseDto { StatusCode = 200, Response = doctor };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
