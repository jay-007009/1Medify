using OneMedify.Infrastructure.Entities;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IPrescriptionMedicineRepository
    {
        Task<PrescriptionMedicine> Create(PrescriptionMedicine prescriptionMedicine);
    }
}
