using OneMedify.DTO.Doctor;
using OneMedify.DTO.Prescription;
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IDoctorActionLogRepository
    {
        List<DoctorActionLog> GetPendingDoctorActionLogs();
        Task<DoctorActionLog> Create(DoctorActionLog doctorActionLog);
        Task<bool> IsDoctorMobileNumberExistsAsync(string mobileNumber);
        Task<List<DoctorActionLog>> GetPendingPrescriptions();
        List<Doctorcount10> DoctorIdCount();
        Task<List<DoctorActionLog>> GetPendingPrescriptionById(int prescriptionId);
        void Delete(DoctorActionLog doctorActionLog);
        DoctorActionLog GetPendingPrescription(int prescriptionId, int doctorId);
        Task<List<Dto>> GetDoctorListForCreatedPrescription(int doctorId);
        Task<List<Dto>> GetDoctorListForUploadedPrescription();
    }
}
