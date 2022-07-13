using Moq;
using OneMedify.DTO.DoctorActionLog;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Services.Services;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class DoctorActionLogServiceTest
    {
        private readonly Mock<IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService> _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService;
        private readonly DoctorActionLogService _doctorActionLogService;
        private readonly Mock<IDoctorActionLogService> _mockDoctorActionLogService;
        private readonly Mock<IDoctorActionLogRepository> _mockDoctorActionLogRepository;
        private readonly MockDoctorControllerData _mockDoctorControllerData;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly Mock<IEmailService> _mockEmailService;


        private readonly Mock<IPrescriptionListSentForApprovalByPharmacy> _mockPrescriptionListSentForApprovalByPharmacy;
        
        public DoctorActionLogServiceTest()
        {
            _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService = new Mock<IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService>();
            _mockDoctorActionLogService = new Mock<IDoctorActionLogService>();
            _mockDoctorControllerData = new MockDoctorControllerData();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockDoctorActionLogRepository = new Mock<IDoctorActionLogRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockPrescriptionListSentForApprovalByPharmacy = new Mock<IPrescriptionListSentForApprovalByPharmacy>();
            _doctorActionLogService = new DoctorActionLogService(_mockPrescriptionListSentForApprovalByPharmacy.Object,
                                                                  _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService.Object,
                                                                  _mockDoctorRepository.Object, _mockDoctorActionLogRepository.Object,
                                                                  _mockEmailService.Object, _mockPrescriptionRepository.Object);
        }

        /// <summary>
        /// Get Prescription List Sent For Approval By Pharmacy Using Doctor Mobile Number (Service Test)
        /// </summary>
        /// <param name="patientMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetPrescriptionsAsyncTestInlineData))]
        public async Task GetPrescriptionListSentForApprovalByPharmacyAsync_WhenCalled_ReturnsResponseDtoAsync(string patientMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockPrescriptionListSentForApprovalByPharmacy.Setup(x => x.GetPrescriptionListSentForApprovalByPharmacy(patientMobileNumber)).Returns(expectedResult);

            var result = await _doctorActionLogService.GetPrescriptionListSentForApprovalByPharmacyAsync(patientMobileNumber);

            Assert.True(result.Response == expectedResult.Result.Response);

        }
        public static IEnumerable<object[]> GetPrescriptionsAsyncTestInlineData =>
        new List<object[]>
           {
                new object[] { "0123456", Task.FromResult(new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidMobileFormat }) }, //When mobileNumbr is invalid
                new object[] { "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber }) }, //When mobile number does not exist.
                new object[] {"0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPharmacyDto> { new PrescriptionByPharmacyDto(), new PrescriptionByPharmacyDto() } }) }, //Success with records
                new object[] { "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PrescriptionByPharmacyDto>() }) } //Success with empty list
           };



        /// <summary>
        /// Get Prescription List Sent For Approval By Patient Using Doctor Mobile Number (Service Test)
        /// </summary>
        /// <param name="doctorMobile"></param>
        /// <param name="isMobileValid"></param>
        /// <param name="isPageIndexValid"></param>
        [Theory]
        [InlineData("9999999999", false, false)]
        [InlineData("9999999999", true, false)]
        [InlineData("9999999999",true, true)]
        public async void To_Check_Response_PrescriptionListSentForApprobalByPatientByDoctorMobileAsync(string doctorMobile, bool isMobileValid, bool isPageIndexValid)
        {

            _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService.Setup(x => x.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctorMobile))
                                                 .Returns(isMobileValid ? (isPageIndexValid ? _mockDoctorControllerData.GetPrescriptionsList_SentForApprovalByPatient_SuccessResponse()
                                                                                                       : _mockDoctorControllerData.GetPrescriptionsList_SentForApprovalByPatient_InvalidPageIndex_Response())
                                                                                                       : _mockDoctorControllerData.GetPrescriptionsList_SentForApprovalByPatient_InvalidDoctorMobileNumber_Response());
            await _doctorActionLogService.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctorMobile);
            _mockDoctorActionLogService.Verify();
        }
    } 
}
