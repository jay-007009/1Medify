using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class DiseaseControllerTest
    {
        private readonly Mock<IDieasesService> _mockDiseaseService;
        private readonly Mock<IMedicineService> _mockMedicineService;
   
        private readonly DiseaseController _diseaseController;
        private readonly MockDiseaseControllerData _mockDiseaseControllerData;

        public DiseaseControllerTest()
        {
            _mockDiseaseService = new Mock<IDieasesService>();
            _mockMedicineService = new Mock<IMedicineService>();
            _diseaseController = new DiseaseController(_mockDiseaseService.Object, _mockMedicineService.Object);
            _mockDiseaseControllerData = new MockDiseaseControllerData();

        }

        /// <summary>
        /// Author: Ketan Singh
        /// Testing Of Controller of GetDisease Responses
        /// </summary>
        /// <param name="isSuccess"></param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Get_Diseases(bool isSuccess)
        {
            //Arrange
            _mockDiseaseService.Setup(x => x.GetDiseases()).Returns(isSuccess ? _mockDiseaseControllerData.GetDiseaseSuccessResponse() : _mockDiseaseControllerData.GetDiseaseInternalServerResponse());
            //Act
            var result = _diseaseController.GetDiseases();
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}