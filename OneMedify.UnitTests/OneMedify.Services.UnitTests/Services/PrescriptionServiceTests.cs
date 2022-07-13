using Moq;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Services.Services;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<ICreatePrescriptionService> _mockCreatePrescription;
        private readonly Mock<IPrescriptionService> _mockPrescriptionService;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly Mock<IPharmacyRepository> _mockPharmacyRepository;
        private readonly Mock<IDiseaseRepository> _mockDiseaseRepository;
        private readonly Mock<IGetApprovedPrescriptionService> _mockGetApprovedPrescriptionService;
        private readonly Mock<IDoctorActionLogService> _mockDoctorActionLogService;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IUploadPrescriptionRepository> _mockUploadPrescriptionRepository;
        private readonly PrescriptionService _prescriptionService;
        private readonly MockPrescriptionData _mockPrescriptionData;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IGetCreatedPrescriptionDetailsByPrescriptionId> _mockGetCreatedPrescriptionDetailsByPrescriptionId;
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IDoctorChangeForPrescriptionService> _mockDoctorChange;
        private readonly Mock<ISendForApprovalService> _mockSendForApprovalService;
        private readonly Mock<IEmailService> _mockEmailService;
        public PrescriptionServiceTests()
        {
            _mockCreatePrescription = new Mock<ICreatePrescriptionService>();
            _mockPharmacyRepository = new Mock<IPharmacyRepository>();
            _mockPrescriptionService = new Mock<IPrescriptionService>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockDoctorActionLogService = new Mock<IDoctorActionLogService>();
            _mockDiseaseRepository = new Mock<IDiseaseRepository>();
            _mockGetApprovedPrescriptionService = new Mock<IGetApprovedPrescriptionService>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockGetCreatedPrescriptionDetailsByPrescriptionId = new Mock<IGetCreatedPrescriptionDetailsByPrescriptionId>();
            _mockFileService = new Mock<IFileService>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockUploadPrescriptionRepository = new Mock<IUploadPrescriptionRepository>();
            _mockSendForApprovalService = new Mock<ISendForApprovalService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockPrescriptionData = new MockPrescriptionData();
            _prescriptionService = new PrescriptionService(
                                                           _mockPrescriptionRepository.Object,
                                                           _mockPatientRepository.Object,
                                                           _mockGetApprovedPrescriptionService.Object,
                                                           _mockCreatePrescription.Object,
                                                           _mockPharmacyRepository.Object,
                                                           _mockDiseaseRepository.Object,
                                                           _mockUploadPrescriptionRepository.Object,
                                                           _mockFileUpload.Object,
                                                           _mockFileValidations.Object,
                                                           _mockDoctorActionLogService.Object,
                                                           _mockDoctorRepository.Object,
                                                           _mockUserValidations.Object,
                                                           _mockFileService.Object,
                                                           _mockGetCreatedPrescriptionDetailsByPrescriptionId.Object,
                                                           _mockSendForApprovalService.Object,
                                                           _mockEmailService.Object);
        }

        /// <summary>
        /// Testing for Get Last 30 Days PrescriptionCount List By Doctor MobileNumber
        /// </summary>
        /// <param name="IsValidMobileNumber"></param>
        /// <param name="doctorMobileNumber"></param>
        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "9854671222")]
        [InlineData(true, "9854671222")]
        public async void To_Get_PrescriptionCountListByMobileNUmber(bool IsValidMobileNumber, string doctorMobileNumber)
        {
            //Arrange
            var prescription = _mockPrescriptionData.GetFakeListOfPrescriptionCountByMobileNumber();
            _mockPrescriptionRepository.Setup(x => x.IsMobileNumberValid(doctorMobileNumber)).Returns(IsValidMobileNumber);
            _mockPrescriptionRepository.Setup(x => x.GetPrescriptionsCountAsync(It.IsAny<string>())).Returns(prescription);
            //Act
            await _prescriptionService.GetPrescriptionsCountAsync(doctorMobileNumber);
            //Assert
            _mockPrescriptionService.Verify();
        }

        /// <summary>
        /// Create prescription unit test.
        /// </summary>
        [Theory, MemberData(nameof(CreatePrescriptionTestInlineData))]
        public async Task CreatePrescriptionAsync_ReturnsResponseDtoAsync(ResponseDto expectedResponseDto, bool isSuccessFull)
        {
            _mockCreatePrescription.Setup(x => x.CreatePrescription(It.IsAny<PrescriptionCreateDto>())).Returns(isSuccessFull ? _mockPrescriptionData.Get_Success_Response() : _mockPrescriptionData.Get_Failure_Response());

            var result = await _prescriptionService.CreatePrescriptionAsync(It.IsAny<PrescriptionCreateDto>());

            _mockPrescriptionService.Verify();

        }
        //Complex object to pass as parameter in CreatePrescription_ReturnsResponseDto for mock response
        public static IEnumerable<object[]> CreatePrescriptionTestInlineData =>
        new List<object[]>
        {
            new object[] { new ResponseDto { StatusCode = 200 }, true },
            new object[] { new ResponseDto { StatusCode = 400 }, false }
        };

        /// <summary>
        /// Get patient's approved and pending prescription by patient's mobile number unit test
        /// </summary>
        [Theory, MemberData(nameof(GetApprovedAndPendingPrescriptionsAsyncTestInlineData))]
        public async Task GetApprovedAndPendingPrescriptionsAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, Task<ResponseDto> expectedResult)
        {
            _mockGetApprovedPrescriptionService.Setup(x => x.GetPatientApprovedAndPendingPrescriptions(pageIndex, patientMobileNumber)).Returns(expectedResult);

            var result = await _prescriptionService.GetApprovedAndPendingPrescriptionsAsync(pageIndex, patientMobileNumber);

            Assert.True(result.Response == expectedResult.Result.Response);
            Assert.True(result.StatusCode == expectedResult.Result.StatusCode);
        }
        //Complex object to pass as parameter in GetPrescriptionsAsync_WhenCalled_ReturnsResponseDtoAsync for mock input and response
        public static IEnumerable<object[]> GetApprovedAndPendingPrescriptionsAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex })   }, //When pageIndex is invalid
            new object[] { 0, "0123456", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidMobileFormat }) }, //When mobileNumbr is invalid
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile }) }, //When mobile number does not exist.
            new object[] { 0, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PatientPrescriptionDto> { new PatientPrescriptionDto(), new PatientPrescriptionDto() } }) }, //Success with records
            new object[] { 1, "0123456789", Task.FromResult(new ResponseDto { StatusCode = 200, Response = new List<PatientPrescriptionDto>() }) } //Success with empty list
        };


        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for GetPharmacyPrescriptionBtPharmacyByMobileNumberAsync Method of Prescription Service 
        /// </summary>
        /// <param name="isMobileNoValid"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="pageindex"></param>
        [Theory]
        [InlineData(true, null, 0)]
        [InlineData(false, "4534663224", 1)]
        [InlineData(true, "4534663224", 1)]
        public async void To_Check_GetPatientPrescriptionByPharmacyMobileNumberAsync(bool isMobileNoValid, string mobileNumber, int pageindex)
        {
            //Arrange
            var prescription = _mockPrescriptionData.Get_PatientPrescriptionByMobileNumber();
            _mockUserValidations.Setup(x => x.IsMobileNumberValid(mobileNumber)).Returns(isMobileNoValid);
            _mockPrescriptionRepository.Setup(x => x.GetAllPatientPrescriptionByPharmacyMobileNumber(It.IsAny<string>())).Returns(prescription);
            //Act
            await _prescriptionService.GetAllPatientPrescriptionByPharmacyMobileNumberAsync(pageindex, mobileNumber);
            //Assert
            _mockPrescriptionService.Verify();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for GetUploadedPrescriptionByPrescriptionIdAsync Method of Prescription Service 
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// 
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async void To_Check_GetPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            //Arrange
            var prescription = _mockPrescriptionData.Get_UploadedPrescriptionByPrescriptionId_Response();
            _mockPrescriptionRepository.Setup(x => x.GetPrescriptionByPrescriptionIdAsync(It.IsAny<int>())).Returns(prescription);
            //Act
            await _prescriptionService.GetPrescriptionByPrescriptionIdAsync(prescriptionId);
            //Assert
            _mockPrescriptionService.Verify();
        }

        /// <summary>
        /// Author: Jay Chauhan
        /// </summary>
        /// <param name="isValidprescriptionId"></param>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(true, false, 0)]
        [InlineData(true, false, 2)]
        [InlineData(false, false, 2)]
        public async Task Get_Created_Prescription_By_PrescriptionId_ReturnsResponseAsync(bool isValidprescriptionId, bool isNull, int prescriptionId)
        {
            var prescription = _mockPrescriptionData.Get_Created_Prescription_By_PrescriptionId_Response();
            var nullPrescription = _mockPrescriptionData.Get_Null_Created_Prescription_By_PrescriptionId_Response();
            //Arrange
            _mockPrescriptionRepository.Setup(x => x.IsCreatedPrescriptionIdExist(prescriptionId)).Returns(isValidprescriptionId);
            _mockPrescriptionRepository.Setup(x => x.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId)).Returns(isNull ? nullPrescription : prescription);
            //Act
            await _prescriptionService.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
            //Assert
            _mockPrescriptionService.Verify();
        }



        /// <summary>
        /// Method to get all patient's created prescription by doctor which are sent for approval by pharmacy unit test.
        /// </summary>
        [Fact]
        public async Task GetCreatedPrescriptionSentForApprovalByPharmacy_InvalidPatienetMobileNumberIsPassed_ReturnsResponseDto()
        {
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_NullPatient_Response());

            var result = await _prescriptionService.GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(It.IsAny<int>(), It.IsAny<string>());

            Assert.True(result.Response == PrescriptionResource.PatientNotExistByMobile);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCreatedPrescriptionSentForApprovalByPharmacy_WhenCalled_ReturnsResponseDtoAsync(bool prescriptionData)
        {
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_Patient_Response());
            _mockPrescriptionRepository.Setup(x => x.GetCreatedPrescriptionSentForApprovalByPharmacy(It.IsAny<string>())).Returns(prescriptionData ? _mockPrescriptionData.Get_PrescriptionList_Response() : _mockPrescriptionData.Get_NullPrescriptionList_Response());

            var result = await _prescriptionService.GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(It.IsAny<int>(), It.IsAny<string>());

            Assert.True(prescriptionData ? result.Response.Count != 0 : result.Response.Count == 0);
        }


        [Fact]
        public async void UploadPrescriptionsByPatient_WhenCalled_CheckResponseAsync()
        {
            var uploadPresriptionDto = _mockPrescriptionData.MockUploadPrescriptionDto();
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_Patient_Response());
            _mockDiseaseRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(_mockPrescriptionData.Get_Disease_Response());
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileValidations.Setup(x => x.ValidateFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>());
            _mockFileUpload.Setup(x => x.GetLengthOfFile(It.IsAny<string>())).Returns(It.IsAny<int>());
            _mockPrescriptionRepository.Setup(x => x.Create(It.IsAny<Prescription>())).Returns(_mockPrescriptionData.Get_Prescription_Response());
            _mockFileUpload.Setup(x => x.GetFileName(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileUpload.Setup(x => x.GetFilePath(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileValidations.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>());
            _mockUploadPrescriptionRepository.Setup(x => x.Create(It.IsAny<PrescriptionUpload>())).Returns(It.IsAny<PrescriptionUpload>());
            _mockDoctorActionLogService.Setup(x => x.GetDoctorsLoop(It.IsAny<Prescription>())).ReturnsAsync(It.IsAny<ResponseDto>());

            var result = await _prescriptionService.UploadPrescriptionByPatient(uploadPresriptionDto);

            _mockPrescriptionService.Verify();
        }

        [Fact]
        public async void UploadPrescriptionsByPharmacy_WhenCalled_CheckResponseAsync()
        {
            var UploadPatientPrescriptionDto = _mockPrescriptionData.MockUploadPatientPrescriptionDto();
            _mockPharmacyRepository.Setup(x => x.GetPharmacyByMobileNumberAsync(It.IsAny<string>())).Returns(_mockPrescriptionData.GetFakePharmacyByMobileNumber());
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_Patient_Response());
            _mockDiseaseRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(_mockPrescriptionData.Get_Disease_Response());
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileValidations.Setup(x => x.ValidateFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>());
            _mockFileUpload.Setup(x => x.GetLengthOfFile(It.IsAny<string>())).Returns(It.IsAny<int>());
            _mockPrescriptionRepository.Setup(x => x.Create(It.IsAny<Prescription>())).Returns(_mockPrescriptionData.Get_Prescription_Response());
            _mockFileUpload.Setup(x => x.GetFileName(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileUpload.Setup(x => x.GetFilePath(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileValidations.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>());
            _mockUploadPrescriptionRepository.Setup(x => x.Create(It.IsAny<PrescriptionUpload>())).Returns(It.IsAny<PrescriptionUpload>());
            _mockDoctorActionLogService.Setup(x => x.GetDoctorsLoop(It.IsAny<Prescription>())).ReturnsAsync(It.IsAny<ResponseDto>());
            var result = await _prescriptionService.UploadPrescriptionByPharmacy(UploadPatientPrescriptionDto);
            _mockPrescriptionService.VerifyAll();
        }

        [Fact]
        public async void UpdatePrescriptionStatus_WhenCalled_CheckResponse()
        {
            var DoctorActionDto = _mockPrescriptionData.MockDoctorActionLog();
            _mockPrescriptionRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(_mockPrescriptionData.GetPrescriptionById_Response());
            _mockDoctorRepository.Setup(x => x.GetDoctorAsync(It.IsAny<string>())).Returns(_mockPrescriptionData.Mock_DoctorAsync());
            _mockPrescriptionRepository.Setup(x => x.UpdatePrescription(It.IsAny<Prescription>())).Returns(_mockPrescriptionData.Mock_UpdatePrescriptionStatus());
            _mockDoctorActionLogService.Setup(x => x.UpdateDoctorAction(It.IsAny<Prescription>())).ReturnsAsync(It.IsAny<ResponseDto>());
            var result = await _prescriptionService.UpdatePrescriptionStatus(DoctorActionDto, 1);
            _mockPrescriptionService.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(SendForApprovalAsyncTestData))]
        public async void ToCheck_SendForApprovalAsync_PassedSuccessfully(int prescriptionId, bool isPrescriptionEmpty, bool isPrescriptionUpdated)
        {
            _mockPrescriptionRepository.Setup(x => x.GetPrescriptionsAsync(prescriptionId)).Returns(isPrescriptionEmpty ?_mockPrescriptionData.Mock_EmptyPrescriptionAsync() : _mockPrescriptionData.Mock_PrescriptionAsync());
            _mockPrescriptionRepository.Setup(x => x.SendForApprovalAsync(_mockPrescriptionData.Mock_Prescription())).Returns(isPrescriptionUpdated ? _mockPrescriptionData.Mock_TrueAsync() : _mockPrescriptionData.Mock_FalseAsync());
            await _prescriptionService.SendForApprovalAsync(prescriptionId, _mockPrescriptionData.Mock_SendForApproval());
            _mockSendForApprovalService.Verify();
        }

        public static IEnumerable<object[]> SendForApprovalAsyncTestData => new List<object[]>
        {
            new object[] { 2, true, true},
            new object[] { 3, false, false},
            new object[] { 5, false, true},
        };
    }
}
