using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorContracts
{
    public interface IGetDoctorsListService
    {
        Task<ResponseDto> GetAllDoctorsAsync(int pageIndex, string doctorName);
    }
}