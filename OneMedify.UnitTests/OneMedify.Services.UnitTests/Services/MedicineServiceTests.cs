using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Services;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class MedicineServiceTests
    {
        private readonly Mock<IMedicineRepository> _mockMedicineRepository;
        private readonly Mock<IMedicineService> _mockMedicineService;
        private readonly MockMedicineData _mockMedicineData;
        private readonly MedicineService _medicineService;
        public MedicineServiceTests()
        {
            _mockMedicineRepository = new Mock<IMedicineRepository>();
            _mockMedicineData = new MockMedicineData();
            _mockMedicineService = new Mock<IMedicineService>();
            _medicineService = new MedicineService(_mockMedicineRepository.Object);
        }
        /// <summary>
        /// unit testing for service layer
        /// </summary>

        [Fact]
        public void To_Check_Get_Empty_Medicines_ByDiseasesIds()
        {
            //Arrange
            var diseaseId = It.IsAny<List<int>>();
            var medicines = _mockMedicineRepository.Setup(medicine => medicine.GetMedicinesByDiseasesIdsAsync(diseaseId)).ReturnsAsync(new List<Medicine>());
            //Act
            var result = _medicineService.GetMedicinesByDiseasesIdsAsync(diseaseId);
            //Assert
            _mockMedicineService.Verify();
        }
        [Fact]
        public void To_Check_Get_All_Medicines_ByDiseasesIds()
        {
            //  Arrange
            var diseaseId = It.IsAny<List<int>>();
            var cities = _mockMedicineRepository.Setup(medicine => medicine.GetMedicinesByDiseasesIdsAsync(diseaseId)).ReturnsAsync(_mockMedicineData.GetAllMedicineByDiseasesIds(diseaseId));
            // Act
            var result = _medicineService.GetMedicinesByDiseasesIdsAsync(diseaseId);
            // Assert
            _mockMedicineService.Verify();
        }

    }
}
