using OneMedify.Infrastructure.Entities;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IPatientDiseaseRepository
    {
        PatientDisease CreatePatientDisease(PatientDisease patientDisease);

        //Read by patient id and disease id
        Task<PatientDisease> ReadById(int patientId, int diseaseId);

        //Update
        Task<PatientDisease> Update(PatientDisease patientDisease);

        Task<PatientDisease> ReadByIdSoftDeleted(int patientId, int diseaseId);

    }
}
