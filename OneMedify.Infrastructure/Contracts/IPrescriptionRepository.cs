using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> Create(Prescription prescription);
        Task<List<Prescription>> GetPrescriptionsCountAsync(string doctorMobileNumber);
        bool IsMobileNumberValid(string doctorMobileNumber);

        /// <summary>
        /// Get Created Prescription by Prescription Id
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        Task<Prescription> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId);

        Task<Prescription> ReadById(int prescriptionId);
        Task<List<Prescription>> GetPrescriptionsListSentForApprovalByPharmacyAsync(string doctorMobileNumber);
        bool IsCreatedPrescriptionIdExist(int prescriptionId);
        Task<List<Prescription>> GetAllPatientPrescriptionByPharmacyMobileNumber(string pharmacymobilenumber);
        Task<PrescriptionUpload> GetPrescriptionByPrescriptionIdAsync(int prescriptionId);
        bool IsUploadedPrescriptionIdExist(int prescriptionId);
        Task<List<Prescription>> GetApprovedAndPendingPrescriptionByPatientMobileNumber(string patientMobilenumber);
        Task<List<Prescription>> GetCreatedPrescriptionSentForApprovalByPharmacy(string patientMobilenumber);
        Prescription UpdatePrescription(Prescription prescription);

        Task<List<Prescription>> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(int pageIndex, string patientMobileNumber);
        bool IsValidPatientMobileNumber(string patientMobileNumber);
        bool MobileNumbereExists(string patientMobileNumber);
        Task<bool> SendForApprovalAsync(Prescription prescription);
        Task<Doctor> GetDoctorNameAsync(int doctorId);
        Task<string> GetPatientNameAsync(int patientId);
        Task<string> GetPharmacyNameAsync(int pharmacyId);
        Task<List<Doctor>> GetAllDoctorsAsync(int number);
        Task<Prescription> GetPrescriptionsAsync(int prescriptionId);
        Task<Prescription> GetUploadedPrescriptionByPrescriptionIdAsync(int prescriptionId);
        Task<List<Prescription>> GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPatient(string doctorMobileNumber);
        Task<List<Prescription>> GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPharmacy(string doctorMobileNumber);
    }
}
