using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockStateControllerData
    {
        public ResponseDto GetStateSuccessResponse()
        {
            return new ResponseDto
            {
                Response = new List<State>()
                {
                    new State
                    {
                        StateId = 1,
                        StateName = "Gujarat"
                    }
                },
                StatusCode = 200
            };
        }

        public ResponseDto GetStateInternalServerResponse()
        {
            return new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500
            };
        }
    }
}