using Microsoft.EntityFrameworkCore;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Repositories
{
   public class DieasesRepository : IDiseaseRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public DieasesRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// GetAllDieases
        /// </summary>
        public List<Disease> GetDiseases()
        {
            try
            {
                var dieases = _oneMedifyDbContext.Diseases.Select(dieases => new Disease()
                {
                    DiseaseId = Convert.ToInt32(dieases.DiseaseId),
                    DiseaseName = dieases.DiseaseName,
                    IsDisable = false
                }).OrderBy(disease => disease.DiseaseName).ToList();
                return dieases;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Get disease details by disease id. 
        /// </summary>
        public async Task<Disease> ReadById(int diseaseId)
        {
            try
            {
                return await _oneMedifyDbContext.Diseases.FirstOrDefaultAsync(disease => disease.DiseaseId == diseaseId
                                                                              && disease.IsDisable == false);            
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
