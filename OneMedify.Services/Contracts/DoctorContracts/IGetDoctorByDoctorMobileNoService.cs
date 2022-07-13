using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorContracts
{
    public interface IGetDoctorByDoctorMobileNoService
    {
        Task<ResponseDto> GetDoctorByDoctorMobileNoAsync(string mobileNo);
    }
}