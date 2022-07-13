using OneMedify.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Services.Contracts
{
    public interface IMedicineService
    {
        Task<ResponseDto> GetMedicinesByDiseasesIdsAsync(List<int> diseasesId);
    }
}