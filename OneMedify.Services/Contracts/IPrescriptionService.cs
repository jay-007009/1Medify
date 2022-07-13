using OneMedify.DTO.Doctor;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IPrescriptionService
    {
        Task<ResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto prescription);
        Task<ResponseDto> GetPrescriptionsCountAsync(string doctorMobileNumber);
        Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId);
        Task<ResponseDto> GetAllPatientPrescriptionByPharmacyMobileNumberAsync(int pageindex, string pharmacyMobileNumber);
        Task<ResponseDto> GetPrescriptionByPrescriptionIdAsync(int prescriptionId);
        Task<ResponseDto> GetApprovedAndPendingPrescriptionsAsync(int pageIndex, string patientMobileNumber);
        Task<ResponseDto> UploadPrescriptionByPatient(UploadPrescriptionDto uploadPrescriptionDto);
        Task<ResponseDto> GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(int pageIndex, string patientMobileNumber);
        Task<ResponseDto> UploadPrescriptionByPharmacy(UploadPatientPrescriptionDto uploadPatientPrescriptionDto);
        Task<ResponseDto> UpdatePrescriptionStatus(DoctorActionDto doctorActionDto, int prescriptionId);
        Task<ResponseDto> SendForApprovalAsync(int prescriptionId, SendForApprovalDto sendForApprovalDto);
        Task<ResponseDto> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(int pageIndex, string patientMobileNumber);
    }
}
