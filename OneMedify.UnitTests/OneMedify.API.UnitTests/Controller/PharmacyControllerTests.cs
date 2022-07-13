using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class PharmacyControllerTests
    {
        private readonly Mock<IPharmacyService> _mockPharmacyService;
        private readonly MockPharmacyControllerData _mockPharmacyControllerData;
        private readonly MockPharmacyData _mockPharmacyData;
        private readonly PharmacyController _pharmacyController;
        private readonly Mock<IPrescriptionService> _mockPrescriptionService;

        public PharmacyControllerTests()
        {

            _mockPharmacyControllerData = new MockPharmacyControllerData();
            _mockPharmacyData = new MockPharmacyData();
            _mockPharmacyService = new Mock<IPharmacyService>();
            _mockPrescriptionService = new Mock<IPrescriptionService>();
            _pharmacyController = new PharmacyController( _mockPharmacyService.Object, _mockPrescriptionService.Object);
        }

        /// <summary>
        /// Unit Testing of PharmacyController 
        /// </summary>
        [Theory]
        [MemberData(nameof(CreatePharmacyInlineData))]
        public async void To_Check_PharmacyRegistration(bool IsEmailAlreadyExists, bool isEmailValid, bool isMobileValid, bool isCityVerified, bool isFileTypeValid, bool isFileSizeValid, bool isOneAuthorityFailed, bool isPharmacyRegistrationFailed)
        {
            var pharmacySignUp = _mockPharmacyData.GetFakePharmacySignUpDto();
            //Arrange
            _mockPharmacyService.Setup(x => x.PharmacyRegistrationAsync(pharmacySignUp)).Returns(IsEmailAlreadyExists ? _mockPharmacyControllerData.ToCheckPharmacyEmailResponse() :
                                                                                                          isEmailValid ? _mockPharmacyControllerData.ToCheckPharmacyEmailValidResponse() :
                                                                                                          isMobileValid ? _mockPharmacyControllerData.ToCheckPharmacyMobileNumberResponse() :
                                                                                                          isCityVerified ? _mockPharmacyControllerData.ToCheckPharmacyInvalidCityIdResponse() :
                                                                                                          isFileTypeValid ? _mockPharmacyControllerData.ToCheckPharmacyInvalidFileTypeResponse() :
                                                                                                          isFileSizeValid ? _mockPharmacyControllerData.ToCheckPharmacyInvalidFileSizeResponse() :
                                                                                                          isOneAuthorityFailed ? _mockPharmacyControllerData.ToCheckPharmacyOneAuthorityResponse() :
                                                                                                          isPharmacyRegistrationFailed ? _mockPharmacyControllerData.ToCheckRegistrationFailedResponse() :
                                                                                                                                         _mockPharmacyControllerData.ToCheckRegistrationSuccessfullyResponse());
            //Act
            var result = await _pharmacyController.Signup(pharmacySignUp);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
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
        /// Unit Testing of Updating Pharmacy Profile
        /// </summary>
        [Theory]
        [InlineData("4516451422", true, true, true, true, true)]
        [InlineData("4516451422", false, true, true, true, true)]
        [InlineData("4516451422", false, false, true, true, true)]
        [InlineData("4516451422", false, false, false, true, true)]
        [InlineData("4516451422", false, false, false, false, true)]
        [InlineData("4516451422", false, false, false, false, false)]
        public async Task To_Check_UpdateProfileAsync(string mobileNumber, bool isUpdateEmpty, bool isPharmacyEmpty, bool isFileTypeValid, bool isFileSizeValid, bool isSuccess)
        {
            var mockPharmacyUpdateDto = It.IsAny<PharmacyUpdateDto>();

            _mockPharmacyService.Setup(x => x.UpdatePharmacyAsync(mobileNumber, mockPharmacyUpdateDto)).Returns(isUpdateEmpty ? _mockPharmacyControllerData.Mock_PharmacyUpdate_NothingToUpdate_Response() :
                                                                                                                 isPharmacyEmpty ? _mockPharmacyControllerData.Mock_PharmacyUpdate_ValidPharmacy_Response() :
                                                                                                                 isFileTypeValid ? _mockPharmacyControllerData.Mock_DoctorUpdate_InvalidFileExtension_Response() :
                                                                                                                 isFileSizeValid ? _mockPharmacyControllerData.Mock_DoctorUpdate_InvalidFileSize_Response() :
                                                                                                                 isSuccess ? _mockPharmacyControllerData.Mock_PharmacyUpdate_Success_Response() :
                                                                                                                             _mockPharmacyControllerData.Mock_PharmacyUpdate_Failed_Response());

            var result = await _pharmacyController.UpdateProfileAsync(mobileNumber, mockPharmacyUpdateDto);

            Assert.IsAssignableFrom<ObjectResult>(result);
        }



        /// <summary>
        /// Author: Bindiya Tandel
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isMobileNoValid"></param>
        /// <param name="mobileNumber"></param>
        [Theory]
        [InlineData(true, true, "9876574857")]
        [InlineData(false, true, "9876574857")]
        [InlineData(false, false, "9876574857")]
        public async void To_Check_GetPharmacyByMobileNumber(bool isSuccess, bool isMobileNoValid, string mobileNumber)
        {
            //Arrange
            _mockPharmacyService.Setup(x => x.GetPharmacyByMobileNumberAsync(mobileNumber))
                                        .ReturnsAsync(isSuccess ? _mockPharmacyControllerData.ToGetPharmacySuccessfullResponse()
                                                                : isMobileNoValid ? _mockPharmacyControllerData.ToGetPharmacyBadRequestResponse()
                                                                : _mockPharmacyControllerData.ToGetPharmacyFailedResponse());
            //Act
            var result = await _pharmacyController.GetPharmacy(mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }


        /// <summary>
        /// Author: Ketan Singh
        /// Unit Testing Of Controller
        /// Method to check Get uploaded prescription by Pharmacy MobileNumber is valid or not.
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="isPageIndexValid"></param>
        /// <param name="isSuccess"></param>
        [Theory]
        [InlineData("1234567890", 0, false, false, false)]
        [InlineData("1234567890", 0, true, false, false)]
        [InlineData("1234567890", 0, true, true, false)]
        [InlineData("1234567890", 0, true, true, true)]
        public async void To_Check_GetPharmacyUploadedPrescriptionsByPatientMobileNumberAsync_Responses(string mobileNumber, int pageIndex, bool isMobileValid, bool isPageIndexValid, bool isSuccess)
        {
            //Arrange
            _mockPharmacyService.Setup(x => x.GetPharmacyUploadedPrescriptionAsync(mobileNumber, pageIndex)).Returns(isMobileValid ? (isPageIndexValid ?
                                                                                                                    (isSuccess ? _mockPharmacyControllerData.GetPharmacyUploadedPrescription_SuccessResponse() :
                                                                                                                                 _mockPharmacyControllerData.GetPharmacyInternalServerResponse()) :
                                                                                                                                 _mockPharmacyControllerData.GetPharmacy_InvalidPageIndex_Response()) :
                                                                                                                                 _mockPharmacyControllerData.GetPharmacy_InvalidMobileNumber_Response());
           //Act
            var result = await _pharmacyController.GetPharmacyUploadedPrescription_ByPharmacyMobileNumberAsync(mobileNumber,pageIndex);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }


    }
}
