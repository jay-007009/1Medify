using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockCityControllerData
    {
        public ResponseDto GetCitySuccessResponse()
        {
            return new ResponseDto
            {
                Response = new List<City>()
                {
                    new City
                    {
                        CityId = 2,
                        CityName = "Surat"
                    }
                },
                StatusCode = 200
            };
        }

        public ResponseDto GetCityInternalServerResponse()
        {
            return new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500

            };
        }

    }
}
