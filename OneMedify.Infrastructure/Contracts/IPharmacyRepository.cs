using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IPharmacyRepository
    {
        bool PharmacyRegistration(Pharmacy pharmacy);
        bool UpdatePharmacy(Pharmacy pharmacy);
        Task<List<Pharmacy>> GetAllPharmacy(int pageindex);
        bool VerfifyCity(int cityId);
        Task<Pharmacy> GetPharmacyByMobileNumberAsync(string mobilenumber);
        Task<List<PrescriptionUpload>> PharmacyUploadedPrescriptionByPharmacyMobileNumberAsync(string mobileNumber);
        Task<List<Disease>> ReadDiseaseByIds(List<int> diseaseIds);
        bool IsValidMobileNo(string mobileNumber);
        Task<Pharmacy> GetPharmacyAsync(string pharmacymobileNumber);
        Task<bool> UpdatePharmacyProfileAsync(Pharmacy pharmacy);

    }
}
