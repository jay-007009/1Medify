using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PatientContracts
{
    public interface IPatientUpdateService
    {
        Task<ResponseDto> UpdatePatientAsync(string mobileNumber, PatientUpdateDto patientUpdateDto);
    }
}