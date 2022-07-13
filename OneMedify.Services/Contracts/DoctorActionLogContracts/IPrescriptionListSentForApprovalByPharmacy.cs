using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorActionLogContracts
{
    public interface IPrescriptionListSentForApprovalByPharmacy
    {
        Task<ResponseDto> GetPrescriptionListSentForApprovalByPharmacy(string doctorMobileNumber);
    }
}
