using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PatientContracts
{
    public interface IGetPatientsCreatedPrescriptionCount
    {
        Task<ResponseDto> GetPatientsCreatedPrescriptionCountAsync(string mobileNumber);
    }
}