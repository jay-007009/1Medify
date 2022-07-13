using OneMedify.Infrastructure.Entities;

using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData
{
    public class MockDiseaseData
    {
        /// <summary>
        /// Author: Ketan Singh
        /// </summary>
        /// <returns></returns>
        public List<Disease> GetMockListOfAllDisease()
        {
            List<Disease> dieases = new List<Disease>()
            {
                new Disease
                {
                    DiseaseId = 125,
                    DiseaseName = "Typhoid"
                }
            };
            return dieases;
        }

        /// <summary>
        /// Author: Ketan Singh
        /// </summary>
        /// <returns></returns>
        public List<Disease> GetMockListOfEmptyDisease()
        {
            List<Disease> dieases = new List<Disease>()
            {
                new Disease
                {
                }
            };
            return dieases;
        }
    }
}