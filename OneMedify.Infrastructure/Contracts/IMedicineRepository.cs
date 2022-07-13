using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IMedicineRepository
    {
        Task<Medicine> GetMedicineByDiseaseId(int medicineId, int diseaseId);

        Task<List<Medicine>> GetMedicineByDiseaseId(int diseaseId);

        Task<List<Medicine>> GetMedicinesByDiseasesIdsAsync(List<int> diseasesId);

        bool IsValidDiseasesId(List<int> diseasesId);

        Task<Disease> DiseasesIdExists(int diseasesId);
    }
}