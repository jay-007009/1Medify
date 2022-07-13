using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IDoctorRepository
    {
        Task<bool> IsMobileNumberValidAsync(string mobileNumber);
        Task<Doctor> ReadDoctorByMobileNumber(string mobileNumber);
        bool IsValidMobileNumber(string mobileno);
        Task<Doctor> GetDoctorByDoctorMobileNumberAsync(string doctormobileNo);
        Task<bool> AddDoctorAsync(Doctor newDoctor);
        Task<bool> UpdateDoctorAsync(Doctor doctor);
        Task<Doctor> GetDoctorAsync(string mobileNumber);
        Task<bool> UpdateDoctorProfileAsync(Doctor doctor);
        Task<int> GetPatientsCountByDoctorIdAsync(int id);
        Task<int> GetPharmaciesCount();
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<int> GetDoctorsCountAsync();
        List<Doctor> GetTenDoctors();
        Task<Doctor> ReadById(int doctorId);
    }
}