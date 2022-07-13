using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorContracts
{
    public interface IDoctorUpdateService
    {
        Task<ResponseDto> UpdateDoctorAsync(string mobileNumber, DoctorUpdateDto doctorUpdateDto);
    }
}