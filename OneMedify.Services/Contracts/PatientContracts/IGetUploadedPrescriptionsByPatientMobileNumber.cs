using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PatientContracts
{
    public interface IGetUploadedPrescriptionsByPatientMobileNumber
    {
        Task<ResponseDto> GetUploadedPrescriptionByPatientMobileNumberAsync(string mobileNumber, int pageIndex);
    }
}