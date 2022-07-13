using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Services.Services.PharmacyServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PharmacyServices
{
    public class GetUploadedPrescriptionsByPharmacyMobileNumberUnitTests
    {
        private readonly Mock<IPharmacyRepository> _mockPharmacyRepository;
        private readonly Mock<IGetUploadedPrescriptionsByPharmacyMobileNumber> _mockGetUploadedPrescriptionsByPharmacyMobileNumber;
        private readonly Mock<IFileService> _mockFileService;
        private readonly MockPharmacyData _mockPharmacyData;
        private readonly GetUploadedPrescriptionsByPharmacyMobileNumber _getUploadedPrescriptionsByPharmacyMobileNumber;
        public GetUploadedPrescriptionsByPharmacyMobileNumberUnitTests()
        {
            _mockPharmacyRepository = new Mock<IPharmacyRepository>();
            _mockGetUploadedPrescriptionsByPharmacyMobileNumber = new Mock<IGetUploadedPrescriptionsByPharmacyMobileNumber>();
            _mockFileService = new Mock<IFileService>();
            _mockPharmacyData = new MockPharmacyData();
            _getUploadedPrescriptionsByPharmacyMobileNumber = new GetUploadedPrescriptionsByPharmacyMobileNumber(_mockPharmacyRepository.Object, _mockFileService.Object);
        }


        /// <summary>
        /// Author: Ketan Singh
        /// To check UploadedPrescriptionByPharmacy By PharmacyMobileNumber
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="isMobileNumberValid"></param>
        [Theory]
        [MemberData(nameof(GetPharmacyUploadedPrescriptionByPharmacyMobileNumberAsyncTestInlineData))]
        public async void ToCheck_GetUploadedPrescriptionByPharmacyMobileNumberAsync_Responses(int pageIndex, bool isMobileNumberValid)
        {
            _mockPharmacyRepository.Setup(x => x.IsValidMobileNo(It.IsAny<string>())).Returns(isMobileNumberValid);
            _mockPharmacyRepository.Setup(x => x.PharmacyUploadedPrescriptionByPharmacyMobileNumberAsync(It.IsAny<string>())).Returns(_mockPharmacyData.GetMockUploadedPrescriptionByPharmacyNo());
            _mockPharmacyRepository.Setup(x => x.ReadDiseaseByIds(It.IsAny<List<int>>())).Returns(_mockPharmacyData.GetMockDiseases());
            await _getUploadedPrescriptionsByPharmacyMobileNumber.GetUploadedPrescriptionByPharmacyMobileNumberAsync(It.IsAny<string>(), pageIndex);
            _mockGetUploadedPrescriptionsByPharmacyMobileNumber.Verify();
        }

        public static IEnumerable<object[]> GetPharmacyUploadedPrescriptionByPharmacyMobileNumberAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { 0 , false },//when mobile number is invalid
            new object[] { 0, true },//when records with success response
            new object[] { -1, true},//when page index is invalid
            new object[] { 0, false}//when records with empty list
        };
    }
}
