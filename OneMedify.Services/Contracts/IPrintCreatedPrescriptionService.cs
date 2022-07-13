using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IPrintCreatedPrescriptionService
    {
        Task<ResponseDto> PrintPrescriptionAsync(int prescriptionId);
    }
}