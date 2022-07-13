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
    public class MedicineRepository : IMedicineRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public MedicineRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Get Medicine with medicine id which belongs to passed disease id.
        /// </summary>
        public async Task<Medicine> GetMedicineByDiseaseId(int medicineId, int diseaseId)
        {
            try
            {
                return await _oneMedifyDbContext.Medicines.FirstOrDefaultAsync(medicine => medicine.MedicineId == medicineId
                                                                                 && medicine.DiseaseId == diseaseId
                                                                                 && medicine.IsDisable == false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get Medicine by disease id.
        /// </summary>
        public async Task<List<Medicine>> GetMedicineByDiseaseId(int diseaseId)
        {
            try
            {
                return await _oneMedifyDbContext.Medicines.Where(medicine => medicine.DiseaseId == diseaseId && medicine.IsDisable == false).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// GetAllMedicinesByDiseaseIds
        /// </summary>
        public async Task<List<Medicine>> GetMedicinesByDiseasesIdsAsync(List<int> diseasesId)
        {
            try
            {
                return await _oneMedifyDbContext.Medicines.Where(disease => diseasesId.Contains(disease.DiseaseId) && disease.IsDisable == false).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        //To check diseaseid is null or not
        public bool IsValidDiseasesId(List<int> diseasesId)
        {
            if (diseasesId == null)
            {
                return true;
            }
            return false;
        }

        //To check diseaseid is exist or not
        public async Task<Disease> DiseasesIdExists(int diseasesId)
        {
            try
            {
                return await _oneMedifyDbContext.Diseases.FirstOrDefaultAsync(disease => disease.DiseaseId == diseasesId && disease.IsDisable == false);

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}