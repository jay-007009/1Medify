using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts.PrescriptionContracts
{
    public interface ICreatePrescriptionService
    {
        Task<ResponseDto> CreatePrescription(PrescriptionCreateDto prescriptionDto);
    }
}
