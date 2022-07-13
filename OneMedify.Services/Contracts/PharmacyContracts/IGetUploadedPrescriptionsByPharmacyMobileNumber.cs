using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PharmacyContracts
{
    public interface IGetUploadedPrescriptionsByPharmacyMobileNumber
    {
        Task<ResponseDto> GetUploadedPrescriptionByPharmacyMobileNumberAsync(string mobileNumber, int pageIndex);
    }
}
