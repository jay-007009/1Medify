using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PrescriptionContracts
{
    public interface ISendForApprovalService
    {
        Task<ResponseDto> SendForApprovalAsync(int prescriptionId, SendForApprovalDto sendForApprovalDto);
        Task<ResponseDto> SendForApprovalAsync(Prescription prescription);
    }
}
