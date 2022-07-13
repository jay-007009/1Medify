using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData
{
    public class MockMedicineData
    {
        public List<Medicine> GetAllMedicineByDiseasesIds(List<int> diseasesId)
        {
            List<Medicine> medicines = new List<Medicine>();
            new Medicine
            {
                MedicineId = 2,
                MedicineName= "Garenoxacin Mesylate",
                DiseaseId=3,
            };
            return medicines;

        }

        public List<Medicine> GetMockEmptyMedicinesByDiseasesIds(List<int> diseasesId)
        {
            List<Medicine> medicines = new List<Medicine>();
            new Medicine
            {

            };
            return medicines;
        }
    }
}
