using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PrescriptionContracts
{
    public interface IGetApprovedPrescriptionService
    {
        Task<ResponseDto> GetPatientApprovedAndPendingPrescriptions(int pageIndex, string patientMobileNumber);
    }
}
