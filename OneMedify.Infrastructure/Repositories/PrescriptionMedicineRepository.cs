using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Repositories
{
    public class PrescriptionMedicineRepository : IPrescriptionMedicineRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;
        public PrescriptionMedicineRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Craete prescription's medicine.
        /// </summary>
        public async Task<PrescriptionMedicine> Create(PrescriptionMedicine prescriptionMedicine)
        {
            try
            {
                await _oneMedifyDbContext.PrescriptionMedicines.AddAsync(prescriptionMedicine);
                await _oneMedifyDbContext.SaveChangesAsync();
                return prescriptionMedicine;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
