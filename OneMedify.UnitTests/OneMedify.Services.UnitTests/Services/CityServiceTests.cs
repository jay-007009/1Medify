using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Services;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTest.Services
{
    public class CityServiceTests
    {
        private readonly Mock<ICityRepository> _mockCityRepository;
        private readonly Mock<ICityService> _mockCityService;
        private readonly MockCityData _mockCityData;
        private readonly CityService _cityService;
        public CityServiceTests()
        {
            _mockCityRepository = new Mock<ICityRepository>();
            _mockCityData = new MockCityData();
            _mockCityService = new Mock<ICityService>();
            _cityService = new CityService(_mockCityRepository.Object);
        }
        /// <summary>
        /// Unit Testing of Services 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="stateId"></param>
        [Fact]
        public void To_Check_Get_Empty_Cities_ByStateId()
        {
            //Arrange
            var stateId = It.IsAny<int>();
            var cities = _mockCityRepository.Setup(city => city.GetCitiesByStateId(stateId)).Returns(new List<City>());
            //Act
            var result = _cityService.GetCitiesByStateId(stateId);
            //Assert
            _mockCityService.Verify();
        }
        [Fact]
        public void To_Check_Get_All_Cities_ByStateId()
        {
            //Arrange
            var stateId = It.IsAny<int>();
            var cities = _mockCityRepository.Setup(city => city.GetCitiesByStateId(stateId)).Returns(_mockCityData.GetAllCitiesByStateId(stateId));
            //Act
            var result = _cityService.GetCitiesByStateId(stateId);
            //Assert
            _mockCityService.Verify();
        }

    }
}
