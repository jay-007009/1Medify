using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
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
    public class PrescriptionControllerTests
    {
        private readonly Mock<IPrescriptionService> _mockPrescriptionService;
        private readonly PrescriptionController _prescriptionController;
        private readonly MockPrescriptionControllerData _mockPrescriptionControllerData;
        private readonly Mock<IPrintCreatedPrescriptionService> _mockPrintCreatedPrescriptionService;
       
        public PrescriptionControllerTests()
        {
            _mockPrescriptionService = new Mock<IPrescriptionService>();
            _mockPrintCreatedPrescriptionService = new Mock<IPrintCreatedPrescriptionService>();
            _mockPrescriptionControllerData = new MockPrescriptionControllerData();
            _prescriptionController = new PrescriptionController(_mockPrescriptionService.Object,
                                                                 _mockPrintCreatedPrescriptionService.Object);
        }

        /// <summary>
        /// GET:/api/prescription/{patientMobileNumber}  API unit test
        /// </summary>
        [Theory, MemberData(nameof(GetPrescriptionsAsyncTestInlineData))]
        public async Task GetPrescriptionsAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, Task<ResponseDto> expectedResult)
        { 
        
            _mockPrescriptionService.Setup(x => x.GetApprovedAndPendingPrescriptionsAsync(pageIndex, patientMobileNumber)).Returns(expectedResult);

            var result = await _prescriptionController.GetPrescriptionsAsync(pageIndex, patientMobileNumber) as ObjectResult;
           

            Assert.True(result.Value == expectedResult.Result);
        
        }

        /// <summary>
        /// Author: Jay Chauhan
        /// GET: /api/prescription/createdPrescription/{prescriptionId}
        /// </summary>
        [Theory]
        [InlineData(0,false, true)]
        [InlineData(0, true, true)]
        [InlineData(91, true, true)]
        public async void To_Check_GetCreatedPrescriptionByPrescriptionIdAsync_Responses(int prescriptionId, bool isPrescriptionValid, bool isSuccess)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId))
                                    .Returns(isPrescriptionValid ? (isSuccess ? _mockPrescriptionControllerData.GetCreatedPrescriptionByPrescriptionIdSuccessResponse() :
                                                                                _mockPrescriptionControllerData.GetCreatedPrescriptionByPrescriptionIdInternalServerResponse()) :
                                                                                _mockPrescriptionControllerData.GetCreatedPrescriptionByInvalidPrescriptionId_Response());
            //Act
            var result = await _prescriptionController.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Author: Jay Chauhan
        /// GET: /api/prescription/print/{prescriptionId}
        /// </summary>
        [Theory]
        [InlineData(0, false, true)]
        [InlineData(0, true, true)]
        [InlineData(91, true, true)]
        public async void GetCreatedPrescriptionByPrescriptionId_Print_Async_Responses(int prescriptionId, bool isPrescriptionValid, bool isSuccess)
        {
            //Arrange
            _mockPrintCreatedPrescriptionService.Setup(x => x.PrintPrescriptionAsync(prescriptionId))
                                    .Returns(isSuccess ? _mockPrescriptionControllerData.Get_Prescription_By_PrescriptionId_Print_Success_Response() :
                                             isPrescriptionValid  ?_mockPrescriptionControllerData.Get_Prescription_By_PrescriptionId_Print_Bad_Request_Response() :
                                              _mockPrescriptionControllerData.Get_Prescription_By_PrescriptionId_Print_InternalServer_Response());
            //Act
            var result = await _prescriptionController.GetCreatedPrescriptionByPrescriptionIdPrintAsync(prescriptionId);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }


        //Complex object to pass as parameter in GetPrescriptionsAsync_WhenCalled_ReturnsResponseDtoAsync for mock input and response
        public static IEnumerable<object[]> GetPrescriptionsAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex })   }, //When pageIndex is invalid
            new object[] { 0, "0123456", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidMobileFormat }) }, //When mobileNumbr is invalid
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile }) }, //When mobile number does not exist.
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PatientPrescriptionDto> { new PatientPrescriptionDto(), new PatientPrescriptionDto() } }) }, //Success with records
            new object[] { 1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PatientPrescriptionDto>() }) } //Succes with empty list
        };

        /// <summary>
        /// GET:/api/prescription/patientprescription/{pharmacymobilenumber}  API unit test
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isMobileNoValid"></param>
        /// <param name="pageindex"></param>
        /// <param name="mobileNumber"></param>
        [Theory]
        [InlineData(true, true, 1, "9876574857")]
        [InlineData(false, true, 1, "9876574857")]
        [InlineData(false, false, 0, "9876574857")]
        public async void To_Check_GetPatientPresccriptionByMobileNumber(bool isSuccess, bool isMobileNoValid, int pageindex, string mobileNumber)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.GetAllPatientPrescriptionByPharmacyMobileNumberAsync(pageindex, mobileNumber))
                                        .ReturnsAsync(isSuccess ? _mockPrescriptionControllerData.ToGetPatientPrescriptionSuccessfullResponse()
                                                                : isMobileNoValid ? _mockPrescriptionControllerData.ToGetPatientPrescriptionBadRequestResponse()
                                                                : _mockPrescriptionControllerData.ToGetPatientPrescriptionFailedResponse());
            //Act
            var result = await _prescriptionController.GetPatientPrescriptionByPharmacyMobileNumberAsync(pageindex, mobileNumber);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// GET:/api/prescription/uploadedprescription/{prescriptionid} API unit test
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="isPrescriptionIdValid"></param>
        /// <param name="prescriptionId"></param>

        [Theory]
        [InlineData(true, true, 1)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 0)]
        public async void To_Check_GetPrescriptionByPrescriptionId(bool isSuccess, bool isPrescriptionIdValid, int prescriptionId)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.GetPrescriptionByPrescriptionIdAsync(prescriptionId))
                                        .ReturnsAsync(isSuccess ? _mockPrescriptionControllerData.ToGetPatientPrescriptionSuccessfullResponse()
                                                                : isPrescriptionIdValid ? _mockPrescriptionControllerData.ToGetPatientPrescriptionBadRequestResponse()
                                                                : _mockPrescriptionControllerData.ToGetPatientPrescriptionFailedResponse());
            //Act
            var result = await _prescriptionController.GetPrescriptionByPrescriptionIdAsync(prescriptionId);
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async void To_Check_SendForApproval(bool isSuccess, bool isPrescriptionIdValid)
        {
            //Arrange
            _mockPrescriptionService.Setup(x => x.SendForApprovalAsync(It.IsAny<int>(), It.IsAny<SendForApprovalDto>())).Returns(isPrescriptionIdValid ? _mockPrescriptionControllerData.SendForApproval_BadRequestResponse() :
                                                        (isSuccess ? _mockPrescriptionControllerData.SendForApproval_SuccessResponse() : _mockPrescriptionControllerData.SendForApproval_InternalServerResponse()));

            //Act
            var result = await _prescriptionController.SendForApprovalAsync(It.IsAny<int>(), It.IsAny<SendForApprovalDto>());
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
