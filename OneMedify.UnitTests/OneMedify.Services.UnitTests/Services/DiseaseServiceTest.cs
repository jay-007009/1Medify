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
    public class DiseaseServiceTest
    {
        private readonly Mock<IDiseaseRepository> _dieasesRepository;
        private readonly Mock<IDieasesService> _mockDiseaseService;
        private readonly DieasesService _diseaseService;
        private readonly MockDiseaseData _mockDiseaseData;

        public DiseaseServiceTest()
        {
            _dieasesRepository = new Mock<IDiseaseRepository>();
            _mockDiseaseData = new MockDiseaseData();
            _diseaseService = new DieasesService(_dieasesRepository.Object);
            _mockDiseaseService = new Mock<IDieasesService>();
        }

        /// <summary>
        /// Author: Ketan Singh
        /// Testing of Service Layer
        /// </summary>
        [Fact]
        public void To_Check_Get_All_Disease()
        {
            //Arrange
            var diseases = _dieasesRepository.Setup(x => x.GetDiseases()).Returns(_mockDiseaseData.GetMockListOfAllDisease());
            //Act
            var result = _diseaseService.GetDiseases();
            //Assert
            _mockDiseaseService.Verify();
        }
        
        /// <summary>
        /// Author: Ketan Singh
        /// Testing of Service Layer
        /// </summary>
        [Fact]
        public void To_Check_Get_EmptyList_Disease()
        {
            //Arrange
            var diseases = _dieasesRepository.Setup(x => x.GetDiseases()).Returns(new List<Disease>());
            //Act
            var result = _diseaseService.GetDiseases();
            //Assert
            _mockDiseaseService.Verify();
        }
    }
}