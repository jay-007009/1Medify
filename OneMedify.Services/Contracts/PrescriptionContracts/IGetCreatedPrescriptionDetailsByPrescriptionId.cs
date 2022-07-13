using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PrescriptionContracts
{
    public interface IGetCreatedPrescriptionDetailsByPrescriptionId
    {
        Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId);
    }
}
