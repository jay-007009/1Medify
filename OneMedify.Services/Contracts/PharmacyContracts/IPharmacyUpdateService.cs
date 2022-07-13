using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PharmacyContracts
{
    public interface IPharmacyUpdateService
    {
        Task<ResponseDto> UpdatePharmacyAsync(string mobileNumber, PharmacyUpdateDto pharmacyUpdateDto);
    }
}
