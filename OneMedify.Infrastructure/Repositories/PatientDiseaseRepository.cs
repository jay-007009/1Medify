using Microsoft.EntityFrameworkCore;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Repositories
{
    public class PatientDiseaseRepository : IPatientDiseaseRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;
        public PatientDiseaseRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }
        public PatientDisease CreatePatientDisease(PatientDisease patientDisease)
        {
            try
            {
                _oneMedifyDbContext.PatientDiseases.Add(patientDisease);
                _oneMedifyDbContext.SaveChanges();
                return patientDisease;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Update patient disease.
        /// </summary>
        public async Task<PatientDisease> Update(PatientDisease patientDisease)
        {
            try
            {
                _oneMedifyDbContext.PatientDiseases.Update(patientDisease);
                await _oneMedifyDbContext.SaveChangesAsync();
                return patientDisease;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get Patient's Disease by patient id and disease id
        /// </summary>
        public async Task<PatientDisease> ReadById(int patientId, int diseaseId)
        {
            try
            {
                return await _oneMedifyDbContext.PatientDiseases.FirstOrDefaultAsync(patientdisease => patientdisease.PatientId == patientId 
                                                                                     && patientdisease.DiseaseId == diseaseId
                                                                                     && patientdisease.IsDisable == false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get Patient's Disease by patient id and disease id which are soft delete (isDisable == true)
        /// </summary>
        public async Task<PatientDisease> ReadByIdSoftDeleted(int patientId, int diseaseId)
        {
            try
            {
                return await _oneMedifyDbContext.PatientDiseases.FirstOrDefaultAsync(patientdisease => patientdisease.PatientId == patientId
                                                                                     && patientdisease.DiseaseId == diseaseId
                                                                                     && patientdisease.IsDisable == true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
