using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorContracts
{
    public interface IGetDoctorsCountService
    {
        Task<ResponseDto> GetDoctorsCountAsync();
    }
}