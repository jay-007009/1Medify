using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IPatientRepository
    {
        /// <summary>
        /// Patient Registration
        /// </summary>
        Patient CreatePatient(Patient patient);
        Patient UpdatePatient(Patient patient);
        Task<Patient> PatientByMobileNumberAsync(string mobileNumber);
        bool ValiadateDiseaseIds(List<int> diseaseIds);

        /// <summary>
        /// Get Patient Prescription By MobileNumber
        /// </summary>
        Task<ICollection<Prescription>> PatientPrescriptionByPatientMobileNumberAsync(string mobileNumber);
        /// <summary>
        /// validation for Mobile No 
        /// </summary>
        bool IsValidMobileNo(string mobileNumber);
        /// <summary>
        /// Get Patient PrescriptionMedicine By PrescriptionId
        /// </summary>
        Task<IEnumerable<PrescriptionMedicine>> MedicineByPrescriptionIdAsync(int id);
        /// <summary>
        /// Get Patient Medicine Details By PrescriptionId
        /// </summary>
        Task<IEnumerable<Medicine>> PrescriptionMedicineByPrescriptionIdAsync(int id);

        //Read patient by mobile number[with disease]
        Task<Patient> ReadPatientByMobileNumber(string mobileNumber);
        Task<int> GetDoctorIdByDoctoMobileAsync(string mobileNumber);
        Task<List<Prescription>> GetPrescriptionsByDoctorIdAsync(int doctorId);
        Task<bool> IsDoctorMobileNumberExistsAsync(string mobileNumber);
        Task<bool> UpdatePatientProfileAsync(Patient patient);
        Task<Patient> GetPatientAsync(string mobileNumber);
        Task<List<Prescription>> GetPrescriptionStatusByPatientMobileNumberAsync(string patientMobileNumber);
        Task<List<PrescriptionUpload>> UploadedPrescriptionByPharmacyPatientMobileNumberAsync(string patientMobileNumber);
        Task<List<PrescriptionUpload>> PatientsUploadedPrescriptionByPatientMobileNumberAsync(string mobileNumber, int pageIndex);
        Task<List<Disease>> ReadDiseaseByIds(List<int> diseaseIds);
        Task<int> PatientCreatedPrescriptionByPatientMobileNumberAsync(string mobileNumber);
    }
}
