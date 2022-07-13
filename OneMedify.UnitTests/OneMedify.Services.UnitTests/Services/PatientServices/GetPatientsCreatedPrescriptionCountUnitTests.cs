using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Services.Services.PatientServices;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PatientServices
{
    public class GetPatientsCreatedPrescriptionCountUnitTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IGetPatientsCreatedPrescriptionCount> _mockGetPatientsCreatedPrescriptionCount;
        private readonly MockPatientData _mockPatientData;
        private readonly GetPatientsCreatedPrescriptionCount _getPatientsCreatedPrescriptionCount;

        public GetPatientsCreatedPrescriptionCountUnitTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockGetPatientsCreatedPrescriptionCount = new Mock<IGetPatientsCreatedPrescriptionCount>();
            _mockPatientData = new MockPatientData();
            _getPatientsCreatedPrescriptionCount = new GetPatientsCreatedPrescriptionCount(_mockPatientRepository.Object);
        }

        [Theory]
        [MemberData(nameof(GetPatientsCreatedPrescriptionCountTestInlineData))]
        public async void ToCheck_GetUploadedPrescriptionByPatientMobileNumberAsync_Responses(bool isMobileNumberValid)
        {
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(It.IsAny<string>())).Returns(isMobileNumberValid);
            _mockPatientRepository.Setup(x => x.PatientCreatedPrescriptionByPatientMobileNumberAsync(It.IsAny<string>())).Returns(_mockPatientData.GetMockDoctorId());
            await _getPatientsCreatedPrescriptionCount.GetPatientsCreatedPrescriptionCountAsync(It.IsAny<string>());
            _mockGetPatientsCreatedPrescriptionCount.Verify();
        }

        public static IEnumerable<object[]> GetPatientsCreatedPrescriptionCountTestInlineData =>
        new List<object[]>
        {
            new object[] { false },
            new object[] { true },
        };
    }
}