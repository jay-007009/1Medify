using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Services.Services.PatientServices;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PatientServices
{
    public class GetUploadedPrescriptionsByPatientMobileNumberUnitTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IGetUploadedPrescriptionsByPatientMobileNumber> _mockGetUploadedPrescriptionsByPatientMobileNumber;
        private readonly MockPatientData _mockPatientData;
        private readonly GetUploadedPrescriptionsByPatientMobileNumber _getUploadedPrescriptionsByPatientMobileNumber;

        public GetUploadedPrescriptionsByPatientMobileNumberUnitTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockGetUploadedPrescriptionsByPatientMobileNumber = new Mock<IGetUploadedPrescriptionsByPatientMobileNumber>();
            _mockPatientData = new MockPatientData();
            _getUploadedPrescriptionsByPatientMobileNumber = new GetUploadedPrescriptionsByPatientMobileNumber(_mockPatientRepository.Object);
        }

        [Theory]
        [MemberData(nameof(GetPatientUploadedPrescriptionByPatientMobileNumberAsyncTestInlineData))]
        public async void ToCheck_GetUploadedPrescriptionByPatientMobileNumberAsync_Responses(int pageIndex, bool isMobileNumberValid)
        {
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(It.IsAny<string>())).Returns(isMobileNumberValid);
            _mockPatientRepository.Setup(x => x.PatientsUploadedPrescriptionByPatientMobileNumberAsync(It.IsAny<string>(), pageIndex)).Returns(_mockPatientData.GetMockPrescriptionUpload());
            _mockPatientRepository.Setup(x => x.ReadDiseaseByIds(It.IsAny<List<int>>())).Returns(_mockPatientData.GetMockDiseases());
            await _getUploadedPrescriptionsByPatientMobileNumber.GetUploadedPrescriptionByPatientMobileNumberAsync(It.IsAny<string>(), pageIndex);
            _mockGetUploadedPrescriptionsByPatientMobileNumber.Verify();
        }

        public static IEnumerable<object[]> GetPatientUploadedPrescriptionByPatientMobileNumberAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { 0 , false },
            new object[] { 0, true }
        };
    }
}