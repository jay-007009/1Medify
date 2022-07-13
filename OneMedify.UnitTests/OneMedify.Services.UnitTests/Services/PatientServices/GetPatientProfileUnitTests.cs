using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Services.Services.PatientServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PatientServices
{
    public class GetPatientProfileUnitTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IGetPatientProfile> _mockGetPatientProfile;
        private readonly MockPatientData _mockPatientData;
        private readonly GetPatientProfile _getPatientProfile;

        public GetPatientProfileUnitTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockFileService = new Mock<IFileService>();
            _mockGetPatientProfile = new Mock<IGetPatientProfile>();
            _mockPatientData = new MockPatientData();
            _getPatientProfile = new GetPatientProfile(_mockPatientRepository.Object, _mockFileService.Object);
        }

        [Theory]
        [MemberData(nameof(GetPatientProfileByPatientMobileNumberAsyncTestInlineData))]
        public async void ToCheck_GetPatientProfileByPatientMobileNumberAsync_Responses(string mobileNumber, bool isMobileNumberValid)
        {
            var path = @"C:\StaticFiles";
            var file = "iVBOR;mso;miigjm";
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(mobileNumber)).Returns(isMobileNumberValid);
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(mobileNumber)).Returns(_mockPatientData.GetFakePatientEntityDto());
            _mockFileService.Setup(x => x.GetFileFromLocation(path)).Returns(file);
            await _getPatientProfile.GetPatientProfileByPatientMobileNumberAsync(mobileNumber);
            _mockGetPatientProfile.Verify();
        }

        public static IEnumerable<object[]> GetPatientProfileByPatientMobileNumberAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { "9876543210", false },
            new object[] { "9876543210", true }
        };
    }
}
