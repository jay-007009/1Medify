using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IPatientService
    {
        /// <summary>
        /// Calling DAL Function from Infrastructure Layer and Adding response code
        /// </summary>
        Task<ResponseDto> PatientRegistration(PatientSignUpDto patientDto);

        /// <summary>
        /// Method: Get API for Patient's Prescriptions By Patient Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        Task<ResponseDto> GetPatientPrescriptionByMobileNo(string mobileNumber);

        Task<ResponseDto> AddPatientDisease(int patientId, int diseaseId, int doctorId);

        Task<ResponseDto> RemovePatientDisease(int patientId, int diseaseId, int doctorId);

        Task<ResponseDto> GetPatientByMobileNo(string mobileNo);

        Task<ResponseDto> GetPatientsByDoctorMobileAsync(string mobileNo, int pageIndex, string patientName);

        Task<ResponseDto> GetPatientProfileAsync(string mobileNumber);
        Task<ResponseDto> GetPrescriptionStatusByPatientMobileNumberAsync(int pageIndex,string patientMobileNumber);
        Task<ResponseDto> UploadedPrescriptionByPharmacyPatientMobileNumberAsync(int pageIndex,string patientMobileNumber);
        Task<ResponseDto> GetPatientUploadedPrescriptionAsync(string mobileNumber, int pageIndex);
        /// <summary>
        /// Method: Update Patient Profile 
        /// </summary>
        Task<ResponseDto> UpdatePatientAsync(string mobileNumber, PatientUpdateDto patientUpdateDto);
        Task<ResponseDto> GetPatientsCreatedPrescriptionCountAsync(string mobileNumber);

    }
}
