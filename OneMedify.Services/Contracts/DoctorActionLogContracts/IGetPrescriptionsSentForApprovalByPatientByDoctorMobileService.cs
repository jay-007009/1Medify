using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorActionLogContracts
{
    public interface IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService
    {
        Task<ResponseDto> GetPrescriptionsSentForApprovalByPatientByDoctorMobile(string doctoMobileNumber);
    }
}