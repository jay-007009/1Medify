using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.DoctorContracts
{
    public interface IPatientsPharmacyCountService
    {
        Task<ResponseDto> GetPatientPharmacyCountAsync(string mobileNumber);
    }
}
