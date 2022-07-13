using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PatientContracts
{
    public interface IGetPatientProfile
    {
        Task<ResponseDto> GetPatientProfileByPatientMobileNumberAsync(string mobileNumber);
    }
}