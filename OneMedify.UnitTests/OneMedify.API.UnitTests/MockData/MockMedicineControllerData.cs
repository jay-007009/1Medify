using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockMedicineControllerData
    {
        public ResponseDto GetMedicinesByDiseasesIdsSuccessResponse()
        {
            return new ResponseDto
            {


                Response = new List<Medicine>()
                {
                    new Medicine
                    {
                        MedicineId=8,
                        MedicineName="Atovaquone",
                        DiseaseId = 3,

                    }
                },
                StatusCode = 200
            };
        }

        public ResponseDto GetMedicinesByDiseasesIdsInternalServerResponse()
        {
            return new ResponseDto
            {

                Response = "Internal Server Error",
                StatusCode = 500
            };
        }

        public ResponseDto GetMedicinesByDiseasesIdsNotFound()
        {
            return new ResponseDto
            {

                Response = "Not Found Error",
                StatusCode = 404
            };
        }

        public ResponseDto GetMedicinesByDiseasesIdsBadRequest()
        {
            return new ResponseDto
            {

                Response = "Bad Request Error",
                StatusCode = 400
            };
        }
    }
}
