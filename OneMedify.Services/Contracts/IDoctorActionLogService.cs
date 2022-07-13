using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IDoctorActionLogService
    {
        Task<ResponseDto> GetPrescriptionsSentForApprovalByPatientByDoctorMobile(string doctoMobileNumber);
        Task<ResponseDto> GetPrescriptionListSentForApprovalByPharmacyAsync(string doctorMobileNumber);
        Task<ResponseDto> GetDoctorsLoop(Prescription prescription);
        Task<ResponseDto> UpdateDoctorAction(Prescription prescription);
        Task<ResponseDto> ChangeDoctorIfBusy(Prescription prescription, Doctor doctor);
    }
}