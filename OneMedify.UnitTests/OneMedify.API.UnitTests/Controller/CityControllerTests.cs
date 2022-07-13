using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class CityControllerTests
    {
        private readonly Mock<ICityService> _mockCityService;
        private readonly MockCityControllerData _mockCityData;
        private readonly CityController _cityController;
        public CityControllerTests()
        {

            _mockCityData = new MockCityControllerData();
            _mockCityService = new Mock<ICityService>();
            _cityController = new CityController(_mockCityService.Object);
        }

        /// <summary>
        /// Unit Testing of CityController 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="stateId"></param>
        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 0)]
        public void To_Check_Get_Cities_ByStateId(bool isSuccess, int? stateId)
        {
            //Arrange
            _mockCityService.Setup(x => x.GetCitiesByStateId(stateId)).Returns(isSuccess ? _mockCityData.GetCitySuccessResponse() : _mockCityData.GetCityInternalServerResponse());
            //Act
            var result = _cityController.Get(stateId);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }


    }
}
