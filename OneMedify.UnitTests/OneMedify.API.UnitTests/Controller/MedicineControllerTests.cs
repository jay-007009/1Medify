using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class MedicineControllerTests
    {
        private readonly Mock<IMedicineService> _mockMedicineService;
        private readonly Mock<IDieasesService> _mockDieasesService;
        private readonly MockMedicineControllerData _mockMedicineData;
        private readonly DiseaseController _diseaseController;
        public MedicineControllerTests()
        {

            _mockMedicineData = new MockMedicineControllerData();
            _mockDieasesService = new Mock<IDieasesService>();
            _mockMedicineService = new Mock<IMedicineService>();
            _diseaseController = new DiseaseController(_mockDieasesService.Object,_mockMedicineService.Object);
        }

        /// <summary>
        /// Unit Testing of Controller 
        /// </summary>
        /// <param name="isSuccess"></param>
     
        [Theory]
        [InlineData(true,null)]
        [InlineData(false,null)]
        public async void To_Check_GetMedicines_By_DiseasesIds(bool isSuccess,List<int> diseasesId)
        {
 
            //Arrange
            _mockMedicineService.Setup(x => x.GetMedicinesByDiseasesIdsAsync(diseasesId)).ReturnsAsync(isSuccess ? _mockMedicineData.GetMedicinesByDiseasesIdsSuccessResponse() : _mockMedicineData.GetMedicinesByDiseasesIdsNotFound());
            _mockMedicineService.Setup(x => x.GetMedicinesByDiseasesIdsAsync(diseasesId)).ReturnsAsync(isSuccess ? _mockMedicineData.GetMedicinesByDiseasesIdsSuccessResponse() : _mockMedicineData.GetMedicinesByDiseasesIdsInternalServerResponse());
            _mockMedicineService.Setup(x => x.GetMedicinesByDiseasesIdsAsync(diseasesId)).ReturnsAsync(isSuccess ? _mockMedicineData.GetMedicinesByDiseasesIdsSuccessResponse() : _mockMedicineData.GetMedicinesByDiseasesIdsBadRequest());
            //Act
            var result = await _diseaseController.GetMedicineByDiseasesIdsAsync(diseasesId);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
