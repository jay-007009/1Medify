using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.DoctorContracts;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorUpdateService _doctorUpdateService;
        private readonly IDoctorRegistrationService _doctorRegistrationService;
        private readonly IGetDoctorByDoctorMobileNoService _getDoctorByDoctorMobileNoService;
        private readonly IPatientsPharmacyCountService _patientsPharmacyCountService;
        private readonly IGetDoctorsCountService _getDoctorsCountService;
        private readonly IGetDoctorsListService _getDoctorsListService;
        public DoctorService(IDoctorUpdateService doctorUpdateService, IDoctorRegistrationService doctorRegistrationService,
                             IGetDoctorByDoctorMobileNoService getDoctorByDoctorMobileNoService, IPatientsPharmacyCountService patientsPharmacyCountService,
                             IGetDoctorsCountService getDoctorsCountService, IGetDoctorsListService getDoctorsListService)
        {

            _doctorUpdateService = doctorUpdateService;
            _doctorRegistrationService = doctorRegistrationService;
            _getDoctorByDoctorMobileNoService = getDoctorByDoctorMobileNoService;
            _patientsPharmacyCountService = patientsPharmacyCountService;
            _getDoctorsCountService = getDoctorsCountService;
            _getDoctorsListService = getDoctorsListService;
        }

        /// <summary>
        /// Method for Registering Doctor
        /// </summary>
        /// <param name="doctorSignUp"></param>
        /// <returns></returns>
        public async Task<ResponseDto> DoctorRegistrationAsync(DoctorSignUpDto doctorSignUp)
        {
            return await _doctorRegistrationService.DoctorRegistrationAsync(doctorSignUp);
        }

        /// <summary>
        /// Method for Updating Doctor Profile
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="doctorUpdateDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UpdateDoctorAsync(string mobileNumber, DoctorUpdateDto doctorUpdateDto)
        {
            try
            {
                return await _doctorUpdateService.UpdateDoctorAsync(mobileNumber, doctorUpdateDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.DoctorUpdateFailed };
            }
        }

        /// <summary>
        /// Method for Getting Pharmacies Count and Patient's Count 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPatientsPharmaciesCountAsync(string mobileNumber)
        {
            try
            {
                return await _patientsPharmacyCountService.GetPatientPharmacyCountAsync(mobileNumber);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }

        public async Task<ResponseDto> GetDoctorByDoctorMobileNoAsync(string mobileNo)
        {
            try
            {
                return await _getDoctorByDoctorMobileNoService.GetDoctorByDoctorMobileNoAsync(mobileNo);
            }
            catch 
            {
                return new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber };
            }
        }

        /// <summary>
        /// Method To Get List of All Registered Doctors
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetAllDoctorsAsync(int pageIndex, string doctorName)
        {
            try
            {
                return await _getDoctorsListService.GetAllDoctorsAsync(pageIndex, doctorName);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }

        /// <summary>
        /// Method to Get Count of Registered Doctors
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDto> GetDoctorsCountAsync()
        {
            try
            {
                return await _getDoctorsCountService.GetDoctorsCountAsync();
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }
    }
}