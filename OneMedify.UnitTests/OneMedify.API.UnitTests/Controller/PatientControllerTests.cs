using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IPrescriptionService> _mockPrescriptionService;
        private readonly MockPatientControllerData _mockPatientControllerData;
        private readonly PatientController _mockPatientController;

        public PatientControllerTests()
        {
            _mockPatientService = new Mock<IPatientService>();
            _mockPrescriptionService = new Mock<IPrescriptionService>();
            _mockPatientController = new PatientController(_mockPatientService.Object, _mockPrescriptionService.Object);
            _mockPatientControllerData = new MockPatientControllerData();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Unit testing for Patient Registration
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="patientSignUpDto"></param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void To_Check_Register_PatientAsync(bool isSuccess)
        {
            var patient = new PatientSignUpDto 
            {
                DateOfBirth = "2022-01-02"
            };
            //Arrange
            _mockPatientService.Setup(x => x.PatientRegistration(patient)).Returns(isSuccess ? _mockPatientControllerData.GetPatientSuccessResponse() : _mockPatientControllerData.GetPatientInternalServerResponse());
            //Act
            var result = await _mockPatientController.SignUpAsync(patient);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void To_Check_Getting_PatientByMobileNoAsync()
        {
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientByMobileNo(It.IsAny<string>())).Returns(_mockPatientControllerData.GetPatientByPatientMobileNo_BadRequest());
            //Act
            var result = await _mockPatientController.GetAsync(It.IsAny<string>());
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void To_Check_Getting_PatientByMobileNoAsyncInternalIssue()
        {
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientByMobileNo(It.IsAny<string>())).Returns(_mockPatientControllerData.GetPatientByPatientMobileNo_InternalServerIssue());
            //Act
            var result = await _mockPatientController.GetAsync(It.IsAny<string>());
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void To_Check_Getting_PatientByMobileNoAsyncSuccessResponse()
        {
            var mobileNumber = "1234567890";
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientByMobileNo(mobileNumber)).Returns(_mockPatientControllerData.GetPatientByPatientMobileNo_SuccessResponse());
            //Act
            var result = await _mockPatientController.GetAsync(mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Unit Testing of Updating Patient Profile
        /// </summary>
        [Theory]
        [InlineData("4516451422", true, true, true, true, true)]
        [InlineData("4516451422", false, true, true, true, true)]
        [InlineData("4516451422", false, false, true, true, true)]
        [InlineData("4516451422", false, false, false, true, true)]
        [InlineData("4516451422", false, false, false, false, true)]
        [InlineData("4516451422", false, false, false, false, false)]
        public async Task To_Check_UpdateProfileAsync(string mobileNumber, bool isUpdateEmpty, bool isPatientEmpty, bool isFileTypeValid, bool isFileSizeValid, bool isSuccess)
        {
            var mockPatientUpdateDto = It.IsAny<PatientUpdateDto>();

            _mockPatientService.Setup(x => x.UpdatePatientAsync(mobileNumber, mockPatientUpdateDto)).Returns(isUpdateEmpty ? _mockPatientControllerData.Mock_PatientUpdate_NothingToUpdate_Response() :
                                                                                                                 isPatientEmpty ? _mockPatientControllerData.Mock_PatientUpdate_ValidPatient_Response() :
                                                                                                                 isFileTypeValid ? _mockPatientControllerData.Mock_PatientUpdate_InvalidFileExtension_Response() :
                                                                                                                 isFileSizeValid ? _mockPatientControllerData.Mock_PatientUpdate_InvalidFileSize_Response() :
                                                                                                                 isSuccess ? _mockPatientControllerData.Mock_PatientUpdate_Success_Response() :
                                                                                                                             _mockPatientControllerData.Mock_PatientUpdate_Failed_Response());

            var result = await _mockPatientController.UpdateProfileAsync(mobileNumber, mockPatientUpdateDto);

            Assert.IsAssignableFrom<ObjectResult>(result);
        }

        [Theory]
        [InlineData("1234567890", false, true)]
        [InlineData("1234567890", true, false)]
        [InlineData("1234567890", true, true)]
        public async void To_Check_GetPatientProfileByPatientMobileNumberAsync_Responses(string mobileNumber, bool isMobileValid, bool isSuccess)
        {
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientProfileAsync(mobileNumber)).Returns(isMobileValid ? (isSuccess ? _mockPatientControllerData.GetPatientPrescriptionSuccessResponse() :
                                                                                                                       _mockPatientControllerData.GetPatientInternalServerResponse()) :
                                                                                                                       _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            //Act
            var result = await _mockPatientController.GetPatientProfileByPatientMobileNumberAsync(mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Theory]
        [InlineData("1234567890", 0, false, false, false)]
        [InlineData("1234567890", 0, true, false, false)]
        [InlineData("1234567890", 0, true, true, false)]
        [InlineData("1234567890", 0, true, true, true)]
        public async void To_Check_GetPatientUploadedPrescriptionsByPatientMobileNumberAsync_Responses(string mobileNumber, int pageIndex, bool isMobileValid, bool isPageIndexValid, bool isSuccess)
        {
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientUploadedPrescriptionAsync(mobileNumber, pageIndex)).Returns(isMobileValid ? (isPageIndexValid ?
                                                                                                                                   (isSuccess ? _mockPatientControllerData.GetPatientUploadedPrescriptionSuccessResponse() :
                                                                                                                                                _mockPatientControllerData.GetPatientInternalServerResponse()) :
                                                                                                                                                 _mockPatientControllerData.GetPatient_InvalidPageIndex_Response()) :
                                                                                                                                                _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            //Act
            var result = await _mockPatientController.GetPatients_UploadedPrescription_ByPatientMobileNumberAsync(mobileNumber, pageIndex);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Get List of Prescriptions Status Send For Approval by Patient Mobile Number (Controller UnitTest) 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetPrescriptionsStatusSentForApprovalAsyncTestInlineData))]
        public async Task GetPrescriptionsStatusAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockPatientService.Setup(x => x.GetPrescriptionStatusByPatientMobileNumberAsync(pageIndex, patientMobileNumber)).Returns(expectedResult);

            var result = await _mockPatientController.GetPrescriptionStatusByPatientMobileNumberAsync(pageIndex, patientMobileNumber) as ObjectResult;

            Assert.True(result.Value == expectedResult.Result);
        }

        public static IEnumerable<object[]> GetPrescriptionsStatusSentForApprovalAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "9874661222", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidPageIndex })   }, //When pageIndex is invalid
            new object[] { 0, "5632987410", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber }) }, //When mobile number does not exist.
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionsStatusDto> { new PrescriptionsStatusDto(), new PrescriptionsStatusDto() } }) }, //Success with records
            new object[] { 1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionsStatusDto>() }) } //Succes with empty list
        };

        /// <summary>
        ///  Get List of Uploaded Prescriptions By Pharmacy by Patient Mobile Number (Controller UnitTest) 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetUploadedPrescriptionsByPharmacyAsyncTestInlineData))]
        public async Task GetUploadedPrescriptionsByPharmacyAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockPatientService.Setup(x => x.UploadedPrescriptionByPharmacyPatientMobileNumberAsync(pageIndex, patientMobileNumber)).Returns(expectedResult);

            var result = await _mockPatientController.UploadedPrescriptionStatusByPatientMobileNumberAsync(pageIndex, patientMobileNumber) as ObjectResult;

            Assert.True(result.Value == expectedResult.Result);
        }

       public static IEnumerable<object[]> GetUploadedPrescriptionsByPharmacyAsyncTestInlineData =>
       new List<object[]>
       {
            new object[] { -1, "9874661222", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PatientResources.InvalidPageIndex })   }, //When pageIndex is invalid
            new object[] { 0, "5632987410", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PatientResources.UnregisteredPatientMobileNumber }) }, //When mobile number does not exist.
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<UploadedPrescriptionsByPharmacyDto> { new UploadedPrescriptionsByPharmacyDto(), new UploadedPrescriptionsByPharmacyDto() } }) }, //Success with records
            new object[] { 1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<UploadedPrescriptionsByPharmacyDto>() }) } //Succes with empty list
       };



        /// <summary>
        /// GET: api/patient/createdPrescriptions/sentForApprovalByPharmacy/{patientMobileNumber}  API unit test
        /// API to get all created prescription sended for approval by pharmacy using patient mobile number.
        /// </summary>
        [Theory, MemberData(nameof(GetCreatedPrescriptionSentForApprovalByPharmacyTestInlineData))]
        public async Task GetCreatedPrescriptionSentForApprovalByPharmacy_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockPrescriptionService.Setup(x => x.GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(pageIndex, patientMobileNumber)).Returns(expectedResult);

            var result = await _mockPatientController.GetCreatedPrescriptionSentForApprovalByPharmacy(patientMobileNumber, pageIndex) as ObjectResult;

            var model = result.Value as ResponseDto;
            Assert.True(model.Response == expectedResult.Result.Response);
        }
        //Complex object to pass as parameter in GetCreatedPrescriptionSentForApprovalByPharmacy_WhenCalled_ReturnsResponseDtoAsync for mock input and response
        public static IEnumerable<object[]> GetCreatedPrescriptionSentForApprovalByPharmacyTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex })   }, //When pageIndex is invalid
            new object[] { 0, "0123456", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidMobileFormat }) }, //When mobileNumbr is invalid
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile }) }, //When mobile number does not exist.
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<CreatedPrescriptionsByPharmacyDto> { new CreatedPrescriptionsByPharmacyDto(), new CreatedPrescriptionsByPharmacyDto() } }) }, //Success with records
            new object[] { 1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<CreatedPrescriptionsByPharmacyDto>() }) } //Succes with empty list
        };

        [Theory]
        [InlineData(false, false, false, false, false)]
        [InlineData(true, false, false, false, false)]
        [InlineData(true, true, false, false, false)]
        [InlineData(true, true, true, false, false)]
        [InlineData(true, true, true, true, false)]
        [InlineData(true, true, true, true, true)]
        public async void To_Check_UploadedPrescriptionsByPatientMobileNumberAsync_Responses(bool isMobilevalid, bool isFileTypeValid, bool isFileSizeValid, bool isSuccess, bool isDiseases)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.UploadPrescriptionByPatient(It.IsAny<UploadPrescriptionDto>())).Returns(isMobilevalid ? (isDiseases ? (isFileTypeValid ? (isFileSizeValid ? (isSuccess ?
                                                                                                                                                        _mockPatientControllerData.UploadPrescription_SuccessResponse() :
                                                                                                                                                        _mockPatientControllerData.UploadPrescription_FailureResponse()) :
                                                                                                                                                        _mockPatientControllerData.ToCheckPrescriptionInvalidFileSizeResponse()) :
                                                                                                                                                        _mockPatientControllerData.ToCheckPrescriptionInvalidFileTypeResponse()) :
                                                                                                                                                        _mockPatientControllerData.GetDisease_ByDiseaseInvalid()) :
                                                                                                                                                        _mockPatientControllerData.Patient_InvalidMobileNumber_Response());

            //Act
            var result = await _mockPatientController.UploadPrescription(It.IsAny<UploadPrescriptionDto>());
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Theory]
        [InlineData("1234567890", 0, false, false, false)]
        [InlineData("1234567890", 0, true, false, false)]
        [InlineData("1234567890", 0, true, true, false)]
        [InlineData("1234567890", 0, true, true, true)]
        public async void To_Check_GetApprovedCreatedPrescriptionsByPatientMobileNumberAsync_Responses(string mobileNumber, int pageIndex, bool isMobileValid, bool isPageIndexValid, bool isSuccess)
        {
            //Arrange
           _mockPrescriptionService.Setup(x => x.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(pageIndex,mobileNumber)).Returns(isMobileValid ? (isPageIndexValid ?
                                                                                                                                   (isSuccess ? _mockPatientControllerData.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_SuccessResponse() :
                                                                                                                                                _mockPatientControllerData.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_InternalServerError()) :
                                                                                                                                                 _mockPatientControllerData.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_BadRequest()) :
                                                                                                                                                _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            //Act
            var result = await _mockPatientController.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(pageIndex,mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}