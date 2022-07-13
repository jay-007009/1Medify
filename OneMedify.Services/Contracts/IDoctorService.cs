using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IDoctorService
    {
        Task<ResponseDto> DoctorRegistrationAsync(DoctorSignUpDto doctorSignUp);
        Task<ResponseDto> GetDoctorByDoctorMobileNoAsync(string mobileNo);
        Task<ResponseDto> GetPatientsPharmaciesCountAsync(string mobileNumber);
        Task<ResponseDto> UpdateDoctorAsync(string mobileNumber, DoctorUpdateDto doctorUpdateDto);
        Task<ResponseDto> GetAllDoctorsAsync(int pageIndex, string doctorName);
        Task<ResponseDto> GetDoctorsCountAsync();
    }
}
