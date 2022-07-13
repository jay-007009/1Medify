using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Services.Services;
using OneMedify.Services.Services.PharmacyServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class PharmacyServiceTests
    {
        private readonly Mock<IPharmacyService> _mockPharmacyService;
        private readonly Mock<IPharmacyUpdateService> _mockPharmacyUpdateService;
        private readonly Mock<IPharmacyRegistrationService> _mockPharmacyRegistrationService;
        private readonly Mock<IPharmacyRepository> _mockPharmacyRepository;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly PharmacyService _pharmacyService;
        private readonly PharmacyUpdateService _pharmacyUpdateService;
        private readonly MockPharmacyControllerData _mockPharmacyControllerData;
        private readonly MockPharmacyData _mockPharmacyData;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IGetUploadedPrescriptionsByPharmacyMobileNumber> _mockUploadedPrescriptionsByPharmacyMobileNumber;

        public PharmacyServiceTests()
        {
            _mockPharmacyService = new Mock<IPharmacyService>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockPharmacyUpdateService = new Mock<IPharmacyUpdateService>();
            _mockPharmacyRegistrationService = new Mock<IPharmacyRegistrationService>();
            _mockPharmacyRepository = new Mock<IPharmacyRepository>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockFileService = new Mock<IFileService>();
            _mockUploadedPrescriptionsByPharmacyMobileNumber = new Mock<IGetUploadedPrescriptionsByPharmacyMobileNumber>();
            _pharmacyService = new PharmacyService(_mockPharmacyUpdateService.Object, _mockPharmacyRepository.Object, _mockUserValidations.Object, _mockFileService.Object, _mockPharmacyRegistrationService.Object,_mockUploadedPrescriptionsByPharmacyMobileNumber.Object);
            _mockPharmacyData = new MockPharmacyData();
            _pharmacyUpdateService = new PharmacyUpdateService(_mockPharmacyRepository.Object, _mockFileUpload.Object, _mockFileValidations.Object);
            _mockPharmacyControllerData = new MockPharmacyControllerData();
        }

        /// <summary>
        /// Unit Testing of Service Layer
        /// </summary>
        /// <param name="IsEmailAlreadyExists"></param>
        /// <param name="isEmailValid"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="isCityVerified"></param>
        /// <param name="isFileTypeValid"></param>
        /// <param name="isFileSizeValid"></param>
        /// <param name="isOneAuthorityFailed"></param>
        /// <param name="isPharmacyRegistrationFailed"></param>
        [Theory]
        [MemberData(nameof(CreatePharmacyInlineData))]
        public async void To_Check_RegisterPharmacy(bool IsEmailAlreadyExists, bool isEmailValid, bool isMobileValid, bool isCityVerified, bool isFileTypeValid, bool isFileSizeValid, bool isOneAuthorityFailed, bool isPharmacyRegistrationFailed)
        {
            var pharmacyDto = _mockPharmacyData.GetFakePharmacySignUpDto();
            _mockPharmacyRegistrationService.Setup(x => x.PharmacyRegistrationAsync(pharmacyDto)).Returns(IsEmailAlreadyExists ? _mockPharmacyControllerData.ToCheckPharmacyEmailResponse() :
                                                                                                          isEmailValid ? _mockPharmacyControllerData.ToCheckPharmacyEmailValidResponse() :
                                                                                                          isMobileValid ? _mockPharmacyControllerData.ToCheckPharmacyMobileNumberResponse() :
                                                                                                          isCityVerified ? _mockPharmacyControllerData.ToCheckPharmacyInvalidCityIdResponse() :
                                                                                                          isFileTypeValid ? _mockPharmacyControllerData.ToCheckPharmacyInvalidFileTypeResponse() :
                                                                                                          isFileSizeValid ? _mockPharmacyControllerData.ToCheckPharmacyInvalidFileSizeResponse() :
                                                                                                          isOneAuthorityFailed ? _mockPharmacyControllerData.ToCheckPharmacyOneAuthorityResponse() :
                                                                                                          isPharmacyRegistrationFailed ? _mockPharmacyControllerData.ToCheckRegistrationFailedResponse() :
                                                                                                                                         _mockPharmacyControllerData.ToCheckRegistrationSuccessfullyResponse());

            await _pharmacyService.PharmacyRegistrationAsync(pharmacyDto);
            _mockPharmacyService.Verify();
        }

        public static IEnumerable<object[]> CreatePharmacyInlineData =>
        new List<object[]>
        {
            new object[] { true, false, false, false, false, false, false, false },
            new object[] { false, true, false, false, false, false, false, false },
            new object[] { false, false, true, false, false, false, false, false },
            new object[] { false, false, false, true, false, false, false, false },
            new object[] { false, false, false, false, true, false, false, false },
            new object[] { false, false, false, false, false, true, false, false },
            new object[] { false, false, false, false, false, false, true, false },
            new object[] { false, false, false, false, false, false, false, true },
            new object[] { false, false, false, false, false, false, false, false },
        };

        /// <summary>
        /// Author: Ketan Singh
        /// Unit Testing for service testing of UpdatePharmacyProfile
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="isPharmacyEmpty"></param>
        /// <param name="isPharmacyUpdated"></param>
        [Theory]
        [MemberData(nameof(UpdatePharmacyAsyncTestData))]
        public async void ToCheck_UpdatePharmacyAsync_PassedSuccessfully(string mobileNumber, bool isPharmacyEmpty, bool isPharmacyUpdated, bool isFileValid, bool isFileUploadValid)
        {

            _mockPharmacyRepository.Setup(x => x.GetPharmacyAsync(mobileNumber)).Returns(isPharmacyEmpty ? _mockPharmacyData.Mock_EmptyPharmacyAsync() : _mockPharmacyData.Mock_PharmacyAsync());
            _mockFileValidations.Setup(x => x.ValidateFile(It.IsAny<string>(), It.IsAny<string>())).Returns(isFileValid ? _mockPharmacyData.Valid_File_Response() : _mockPharmacyData.Invalid_File_Response());
            _mockFileValidations.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(isFileUploadValid ? _mockPharmacyData.Valid_UploadFile_Response() : _mockPharmacyData.Invalid_UploadFile_Response());
            _mockFileValidations.Setup(x => x.ValidateFileLength(It.IsAny<string>())).Returns(It.IsAny<ResponseDto>);
            _mockFileUpload.Setup(x => x.DeleteFile(_mockPharmacyData.Mock_Pharmacy().PharmacistDegreeFilePath)).Returns(true);
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>())).Returns(_mockPharmacyData.Mock_FileValidationDto().SecondFileExtension);
            _mockPharmacyRepository.Setup(x => x.UpdatePharmacyProfileAsync(_mockPharmacyData.Mock_Pharmacy())).Returns(isPharmacyUpdated ? _mockPharmacyData.Mock_TrueAsync() : _mockPharmacyData.Mock_FalseAsync());
            await _pharmacyUpdateService.UpdatePharmacyAsync(mobileNumber, _mockPharmacyData.Mock_PharmacyUpdateDto());
            _mockPharmacyUpdateService.Verify();
        }

        public static IEnumerable<object[]> UpdatePharmacyAsyncTestData => new List<object[]>
        {
            new object[] { "7845962154", true, true,true,true},
            new object[] { "7845962154", false, false,false,false},
            new object[] { "7845962154", true, false,true, false},
            new object[] { "7845962154", true, true,false, true},
            new object[] { "7845962154", true, true,true, false}

        };

        [Theory]
        [InlineData(0)]
        public async void ToCheck_AllPharmacyList(int pageIndex)
        {
            var pharmacyName = "om pharmacy";
            //Arrange
            var pharmacy = _mockPharmacyRepository.Setup(x => x.GetAllPharmacy(pageIndex)).Returns(_mockPharmacyData.GetMockPharmacyDto());
            //Act
            var result = await _pharmacyService.GetAllPharmacies(pageIndex, pharmacyName);
            //Assert
            _mockPharmacyService.VerifyAll();
        }

        [Theory]
        [InlineData(0)]
        public async void ToCheck_EmptyPharmacyList(int pageIndex)
        {
            var pharmacyName = "om pharmacy";
            //Arrange
            var pharmacy = _mockPharmacyRepository.Setup(x => x.GetAllPharmacy(pageIndex)).ReturnsAsync(new List<Pharmacy>());
            //Act
            var result = await _pharmacyService.GetAllPharmacies(pageIndex, pharmacyName);
            //Assert
            _mockPharmacyService.VerifyAll();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for GetPharmacyByMobileNumberAsync Method of Pharmacy Service 
        /// </summary>
        /// <param name="isMobileNoValid"></param>
        /// <param name="mobileNumber"></param>
        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "4534663224")]
        [InlineData(true, "4534663224")]
        public async void To_Check_GetPharmacyByMobileNumberAsync(bool isMobileNoValid, string mobileNumber)
        {
            //Arrange
            var pharmacy = _mockPharmacyData.GetFakePharmacyByMobileNumber();
            _mockUserValidations.Setup(x => x.IsMobileNumberValid(mobileNumber)).Returns(isMobileNoValid);
            _mockPharmacyRepository.Setup(x => x.GetPharmacyByMobileNumberAsync(It.IsAny<string>())).Returns(pharmacy);
            //Act
            await _pharmacyService.GetPharmacyByMobileNumberAsync(mobileNumber);
            //Assert
            _mockPharmacyService.Verify();
        }


        /// <summary>
        /// Author: Ketan Singh
        ///  Get List of Uploaded Prescriptions By Pharmacy by Pharmacy Mobile Number 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pharmacyMobileNumber"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetUploadedPrescriptionsByPharmacyAsyncTestInlineData))]
        public async Task GetUploadedPrescriptionByPharmacyAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string pharmacyMobileNumber, bool isValid)
        {
            _mockPharmacyRepository.Setup(x => x.PharmacyUploadedPrescriptionByPharmacyMobileNumberAsync(pharmacyMobileNumber)).Returns(_mockPharmacyData.GetMockUploadedPrescriptionListByPharmacy());
            _mockPharmacyRepository.Setup(x => x.IsValidMobileNo(pharmacyMobileNumber)).Returns(isValid);

            var result = await _pharmacyService.GetPharmacyUploadedPrescriptionAsync(pharmacyMobileNumber, pageIndex);
            _mockPharmacyService.Verify();
        }

        public static IEnumerable<object[]> GetUploadedPrescriptionsByPharmacyAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "0123456789",true }, //When pageIndex is invalid  
            new object[] { 0, "0123456789",false }, //When mobile number does not exist.
            new object[] { 0, "0123456789",true }, //Success with records
            new object[] { 1, "0123456789", false} //Success with empty list
        };
    }
}
