using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IPharmacyService
    {
        Task<ResponseDto> PharmacyRegistrationAsync(PharmacySignUpDto pharmacySignUp);
        Task<ResponseDto> GetPharmacyByMobileNumberAsync(string mobilenumber);
        Task<ResponseDto> GetAllPharmacies(int pageindex, string pharmacyName);
        Task<ResponseDto> UpdatePharmacyAsync(string mobileNumber, PharmacyUpdateDto pharmacyUpdateDto);
        Task<ResponseDto> GetPharmacyUploadedPrescriptionAsync(string mobileNumber, int pageIndex);
    }
}
