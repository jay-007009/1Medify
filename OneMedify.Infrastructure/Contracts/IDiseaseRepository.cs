using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IDiseaseRepository
    {
        /// <summary>
        /// GetAllDieasesList 
        /// </summary>
        List<Disease> GetDiseases();
        Task<Disease> ReadById(int diseaseId);
    }
}
