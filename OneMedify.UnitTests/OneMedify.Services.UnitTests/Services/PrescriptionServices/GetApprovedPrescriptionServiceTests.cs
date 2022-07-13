using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Services.PrescriptionServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PrescriptionServices
{
    public class GetApprovedPrescriptionServiceTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly Mock<IFileService> _mockFileService;
        private readonly MockPrescriptionData _mockPrescriptionData;
        private readonly GetApprovedPrescriptionService _getApprovedPrescriptionService;

        public GetApprovedPrescriptionServiceTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockFileService = new Mock<IFileService>();
            _mockPrescriptionData = new MockPrescriptionData();
            _getApprovedPrescriptionService = new GetApprovedPrescriptionService(_mockPrescriptionRepository.Object, 
                                              _mockPatientRepository.Object, _mockFileService.Object);
        }

        /// <summary>
        /// Unit test for Method to get patient's approved and pending prescriptions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPatientApprovedAndPendingPrescriptions_InvalidPatienetMobileNumberIsPassed_ReturnsResponseDto()
        {
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_NullPatient_Response());
            _mockFileService.Setup(x => x.GetFileFromLocation(It.IsAny<string>())).Returns(It.IsAny<string>());

            var result = await _getApprovedPrescriptionService.GetPatientApprovedAndPendingPrescriptions(It.IsAny<int>(), It.IsAny<string>());

            Assert.True(result.Response == PrescriptionResource.PatientNotExistByMobile);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetPatientApprovedAndPendingPrescriptions_WhenCalled_ReturnsResponseDtoWithPrescriptionData(bool prescriptionData)
        {
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_Patient_Response());
            _mockPatientRepository.Setup(x => x.ReadDiseaseByIds(It.IsAny<List<int>>())).Returns(_mockPrescriptionData.Get_Disesaes_Response());
            _mockPrescriptionRepository.Setup(x => x.GetApprovedAndPendingPrescriptionByPatientMobileNumber(It.IsAny<string>())).Returns(prescriptionData? _mockPrescriptionData.Get_PrescriptionList_Response() : _mockPrescriptionData.Get_NullPrescriptionList_Response());
            _mockFileService.Setup(x => x.GetFileFromLocation(It.IsAny<string>())).Returns(It.IsAny<string>());

            var result = await _getApprovedPrescriptionService.GetPatientApprovedAndPendingPrescriptions(It.IsAny<int>(), It.IsAny<string>());

            Assert.True(prescriptionData ? result.Response.Count != 0 : result.Response.Count == 0);
        }
    }
}
