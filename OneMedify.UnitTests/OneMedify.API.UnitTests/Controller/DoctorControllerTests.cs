using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.DTO.Doctor;
using OneMedify.DTO.DoctorActionLog;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class DoctorControllerTests
    {
        private readonly Mock<IDoctorService> _mockDoctorService;
        private readonly Mock<IPrescriptionService> _mockPrescriptionService;
        private readonly MockDoctorControllerData _mockDoctorControllerData;
        private readonly DoctorController _doctorController;
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IDoctorActionLogService> _mockDoctorActionLogService;

        public DoctorControllerTests()
        {
            _mockDoctorService = new Mock<IDoctorService>();
            _mockPrescriptionService = new Mock<IPrescriptionService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockDoctorActionLogService = new Mock<IDoctorActionLogService>();
            _mockDoctorControllerData = new MockDoctorControllerData();
            _doctorController = new DoctorController(_mockDoctorService.Object, _mockPatientService.Object, _mockPrescriptionService.Object,_mockDoctorActionLogService.Object);
            
        }

        /// <summary>
        /// Author: Ketan Singh
        /// Unit Testing of Controller
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="doctorSignUpDto"></param>
        [Theory]
        [InlineData(true, null)]
        [InlineData(false, null)]
        public async Task To_Check_DoctorRegistrationAsync(bool isSuccess, DoctorSignUpDto doctorSignUpDto)
        {
            //Arrange
            _mockDoctorService.Setup(doctorsignup => doctorsignup.DoctorRegistrationAsync(It.IsAny<DoctorSignUpDto>())).ReturnsAsync(isSuccess ? _mockDoctorControllerData.ToCheck_DoctorRegistrationSuccessResponse() : _mockDoctorControllerData.ToCheck_DoctorRegistrationFailedResponse());
            //Act
            var result = await _doctorController.Signup(doctorSignUpDto);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Author: Ketan Singh
        /// Unit Testing Of Controller
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="mobileno"></param>
        [Theory]
        [InlineData(true, true, "8574961263")]
        [InlineData(false, true, "8574961263")]
        [InlineData(false, false, "8574961263")]
        public void To_Check_DoctorByDoctorMobileNo_Response(bool isSuccess, bool isMobileValid, string mobileno)
        {
            //Arrange
            _mockDoctorService.Setup(doctorprofile => doctorprofile.GetDoctorByDoctorMobileNoAsync(mobileno)).Returns(isSuccess ? _mockDoctorControllerData.ToCheck_GetDoctorByDoctorMobileno_SuccessResponse() : isMobileValid ? _mockDoctorControllerData.ToCheck_GetDoctorByDoctorMobile_DoesNotExists() : _mockDoctorControllerData.ToCheck_GetDoctorByDoctorMobileno_InternalServerIssues());
            //Act
            var result = _doctorController.GetDoctorByMobileNo(mobileno);
            //Assert
            Assert.IsAssignableFrom<IAsyncResult>(result);
        }

        /// <summary>
        /// Method:Get Last 30 Days PrescriptionCount List By Doctor MobileNumber
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="mobileNumber"></param>
        [Theory]
        [InlineData(true, false, null)]
        [InlineData(false, true, "4516451422")]
        [InlineData(true, true, null)]
        [InlineData(true, true, "4516451422")]
        public void To_Check_GetPrescriptionCountListByMobileNumber(bool isSuccess, bool isMobileValid, string mobileNumber)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.GetPrescriptionsCountAsync(mobileNumber)).ReturnsAsync(isSuccess ? _mockDoctorControllerData.To_Get_PrescriptionSuccessResponse() : isMobileValid ? _mockDoctorControllerData.To_Get_PrescriptionBadResponse() : _mockDoctorControllerData.To_Get_PrescriptionFailedResponse());
            //Act
            var result = _doctorController.GetPrescriptionCount(mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IAsyncResult>(result);
        }

        /// <summary>
        /// POST: api/doctor/createprescription unitTest
        /// </summary>
        [Fact]
        public async Task CreatePrescription_WhenCalled_ReturnsResponseDtoAsync()
        {
            var mockPrescriptionData = It.IsAny<PrescriptionCreateDto>();

            _mockPrescriptionService.Setup(x => x.CreatePrescriptionAsync(mockPrescriptionData)).Returns(It.IsAny<Task<ResponseDto>>());

            var result = await _doctorController.CreatePrescription(mockPrescriptionData);
        }

        /// <summary>
        /// Get List Of Patients By Doctor Mobile Number (Controller Test)
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="mobileNo"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(true, true, "9852147521", 0, "test")]
        [InlineData(false, true, "9852147521", 0, "test")]
        [InlineData(false, false, "9852147521", 0, "test")]
        public async Task To_Check_PatientsDetails_ByDoctorMobileAsync_IsSuccess(bool isSuccess, bool isMobileValid, string mobileNo, int pageIndex, string patientName)
        {
            //Arrange
            _mockPatientService.Setup(x => x.GetPatientsByDoctorMobileAsync(mobileNo, pageIndex, patientName))
                                .Returns(isSuccess ? _mockDoctorControllerData.GetMyPatientsByDoctorMobileSuccessResponse()
                                            : isMobileValid ? _mockDoctorControllerData.GetMyPatientsByDoctorMobileBadRequestResponse()
                                            : _mockDoctorControllerData.GetMyPatientsByDoctorMobileInternalServerResponse());
            //Act
            var result = await _doctorController.MyPatients(mobileNo, pageIndex, patientName);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsAssignableFrom<ObjectResult>(result);
        }

        /// <summary>
        /// Unit Testing of Updating Doctor Profile
        /// </summary>
        [Theory]
        [InlineData("4516451422", true, true, true, true, true)]
        [InlineData("4516451422", false, true, true, true, true)]
        [InlineData("4516451422", false, false, true, true, true)]
        [InlineData("4516451422", false, false, false, true, true)]
        [InlineData("4516451422", false, false, false, false, true)]
        [InlineData("4516451422", false, false, false, false, false)]
        public async Task To_Check_UpdateProfileAsync(string mobileNumber, bool isUpdateEmpty, bool isDoctorEmpty, bool isFileTypeValid, bool isFileSizeValid, bool isSuccess)
        {
            var mockDoctorUpdateDto = It.IsAny<DoctorUpdateDto>();

            _mockDoctorService.Setup(x => x.UpdateDoctorAsync(mobileNumber, mockDoctorUpdateDto)).Returns(isUpdateEmpty ? _mockDoctorControllerData.Mock_DoctorUpdate_NothingToUpdate_Response() :
                                                                                                                 isDoctorEmpty ? _mockDoctorControllerData.Mock_DoctorUpdate_ValidDoctor_Response() :
                                                                                                                 isFileTypeValid ? _mockDoctorControllerData.Mock_DoctorUpdate_InvalidFileExtension_Response() :
                                                                                                                 isFileSizeValid ? _mockDoctorControllerData.Mock_DoctorUpdate_InvalidFileSize_Response() :
                                                                                                                 isSuccess ? _mockDoctorControllerData.Mock_DoctorUpdate_Success_Response() :
                                                                                                                             _mockDoctorControllerData.Mock_DoctorUpdate_Failed_Response());

            var result = await _doctorController.UpdateProfileAsync(mobileNumber, mockDoctorUpdateDto);

            Assert.IsAssignableFrom<ObjectResult>(result);
        }

        [Theory]
        [InlineData("4516451422", true, true)]
        [InlineData("4516451422", false, true)]
        [InlineData("4516451422", false, false)]
        public async Task To_Check_GetPharmicesPatientCountAsync(string mobileNumber, bool isMobileValid, bool isSuccess)
        {


            _mockDoctorService.Setup(x => x.GetPatientsPharmaciesCountAsync(mobileNumber)).Returns(isMobileValid ? _mockDoctorControllerData.ToCheck_GetDoctorByDoctorMobile_DoesNotExists() :
                                                                                                                   isSuccess ? _mockDoctorControllerData.Mock_PharmacyPatientCountDto_Response() :
                                                                                                                             _mockDoctorControllerData.GetMyPatientsByDoctorMobileInternalServerResponse());

            var result = await _doctorController.GetPharmicesPatientCount(mobileNumber);

            Assert.IsAssignableFrom<ObjectResult>(result);
        }

        /// <summary>
        /// Get List Of All Registered Doctors(Controller Test)
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="pageIndex"></param>
        /// <param name="doctorName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(true, 0, "test")]
        [InlineData(false, 0, "test")]
        public async Task To_Check_Response_DoctorsListAsync(bool isSuccess, int pageIndex, string doctorName)
        {
            //Arrange
            _mockDoctorService.Setup(x => x.GetAllDoctorsAsync(pageIndex, doctorName)).ReturnsAsync(isSuccess ? _mockDoctorControllerData.Mock_DoctorsList_SuccessResponse()
                                                                                                  : _mockDoctorControllerData.Mock_DoctorList_InternalServerResponse());
            //Act
            var result = await _doctorController.GetDoctorsList(pageIndex, doctorName);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Get Count Of All Registered Doctors(Controller Test)
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task To_Check_Response_DoctorsCountAsync(bool isSuccess)
        {
            //Arrange
            _mockDoctorService.Setup(x => x.GetDoctorsCountAsync()).ReturnsAsync(isSuccess ? _mockDoctorControllerData.Mock_DoctorsCount_SuccessResponse()
                                                                                           : _mockDoctorControllerData.Mock_DoctorsCount_InternalServerResponse());
            //Act
            var result = await _doctorController.GetDoctorsCount();
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Get List Of Prescriptions Sent For Approval By Patient Using DoctorMobileNumber(Controller Test)
        /// </summary>
        /// <param name="doctorMobile"></param>
        /// <param name="expectedResult"></param>
        [Theory, MemberData(nameof(GetUploadedPrescriptionsSentForApprovalByPatientAsyncTestInlineData))]
        public async void To_Check_Response_PrescriptionListSentForApprobalByPatientByDoctorMobileAsync(string doctorMobile, Task<ResponseDto> expectedResult)
        {
            //Arrange
            _mockDoctorActionLogService.Setup(x => x.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctorMobile))
                                                 .Returns(expectedResult);
            var result = await _doctorController.GetPrescriptionsSentForApprovalbyPatient(doctorMobile) as ObjectResult;
            //Assert
            Assert.True(result.Value == expectedResult.Result);
           
        }

        public static IEnumerable<object[]> GetUploadedPrescriptionsSentForApprovalByPatientAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] {"5632987410", Task.FromResult(new ResponseDto { StatusCode = 400, Response = DoctorActionLogResources.UnregisteredDoctorMobileNumber }) }, //When mobile number does not exist.
            new object[] {"0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPatientDto> { new PrescriptionByPatientDto(), new PrescriptionByPatientDto() } }) }, //Success with records
            new object[] {"0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPatientDto>() }) } //Succes with empty list
        };
        /// <summary>
        /// Get PrescriptionList Sent For Approval By Pharmacy Using DoctorMobileNumber(Controller Test)
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetUploadedPrescriptionsSentForApprovalByPharmacyAsyncTestInlineData))]
        public async Task GetUploadedPrescriptionsByPharmacyAsync_WhenCalled_ReturnsResponseDtoAsync(string doctorMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockDoctorActionLogService.Setup(x => x.GetPrescriptionListSentForApprovalByPharmacyAsync(doctorMobileNumber)).Returns(expectedResult);

            var result = await _doctorController.GetPrescriptionListSentForApprovalByPharmacy(doctorMobileNumber) as ObjectResult;

            Assert.True(result.Value == expectedResult.Result);
        }
        public static IEnumerable<object[]> GetUploadedPrescriptionsSentForApprovalByPharmacyAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { "5632987410", Task.FromResult(new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber }) }, //When mobile number does not exist.
            new object[] { "5632987410", Task.FromResult(new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidMobileFormat}) }, //When mobile number is not valid format.
            new object[] { "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPharmacyDto> { new PrescriptionByPharmacyDto(), new PrescriptionByPharmacyDto() } }) }, //Success with records
            new object[] { "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPharmacyDto>() }) } //Succes with empty list
        };



        [Theory]
        [InlineData(9, true, true, true, true, true)]
        [InlineData(9, false, true, true, true, true)]
        [InlineData(9, false, false, true, true, true)]
        [InlineData(9, false, false, false, true, true)]
        [InlineData(9, false, false, false, false, true)]
        [InlineData(9, false, false, false, false, false)]
        public async Task ToCheck_UpdateDoctorAction_WhenCalled(int PrescriptionId, bool isMobileNumber, bool isApprove, bool isReject, bool isBusy, bool isExpire)
        {
            var mockDoctorActionDto = It.IsAny<DoctorActionDto>();

            _mockPrescriptionService.Setup(x => x.UpdatePrescriptionStatus(mockDoctorActionDto, PrescriptionId))
                                    .Returns(isApprove ? _mockDoctorControllerData.UpdateDoctorAction_SuccessfullResponse_IfPrescriptionApprove() :
                                             isReject ? _mockDoctorControllerData.UpdateDoctorAction_SuccessfullResponse_IfPrescriptionReject() :
                                             isBusy ? _mockDoctorControllerData.UpdateDoctorAction_SuccessfullResponse_IfDoctorBusy() :
                                             isMobileNumber ? _mockDoctorControllerData.UpdateDoctorAction_DoctorMobileNumberNotExist() :
                                             _mockDoctorControllerData.UpdateDoctorAction_InternalServerResponse());

            var result = await _doctorController.UpdateDoctorAction(PrescriptionId, mockDoctorActionDto);
            Assert.IsAssignableFrom<ObjectResult>(result);
        }
    }
}