using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PharmacyContracts
{
    public interface IPharmacyRegistrationService
    {
        Task<ResponseDto> PharmacyRegistrationAsync(PharmacySignUpDto pharmacySignUpDto);
    }
}