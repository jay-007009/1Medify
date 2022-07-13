using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PatientContracts
{
    public interface IGetPatientByDoctorMobile
    {
        Task<ResponseDto> GetPatientsByDoctorMobileAsync(string mobileNo, int pageIndex, string patientName);
    }
}