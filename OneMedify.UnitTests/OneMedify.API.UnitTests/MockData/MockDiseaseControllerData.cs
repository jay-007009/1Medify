using OneMedify.DTO.Disease;
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockDiseaseControllerData
    {
        /// <summary>
        /// Author: Ketan Singh
        /// </summary>
        /// <returns></returns>
        public DiseaseResponseDto GetDiseaseSuccessResponse()
        {
            return new DiseaseResponseDto
            {
             

                Response = new List<Disease>()
                {
                    new Disease
                    {
                        DiseaseId = 125,
                        DiseaseName = "Typhoid"
                    }
                },
                StatusCode = 200
            };
        }

        /// <summary>
        /// Author: Ketan Singh
        /// </summary>
        /// <returns></returns>
        public DiseaseResponseDto GetDiseaseInternalServerResponse()
        {
            return new DiseaseResponseDto
            {

                Response = "Internal Server Error",
                StatusCode = 500
            };
        }


        
    }
}