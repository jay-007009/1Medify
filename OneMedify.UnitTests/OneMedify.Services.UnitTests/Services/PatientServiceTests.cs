using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Services.Services;
using OneMedify.Services.Services.PatientServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IPatientDiseaseRepository> _mockPatientDiseaseRepository;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IOneAuthorityService> _mockOneAuthorityService;
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IFileService> _mockFileService;
        private readonly PatientUpdateService _patientUpdateService;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IPatientUpdateService> _mockPatientUpdateService;
        private readonly Mock<IPharmacyRepository> _mockPharmacyRepository;
        private readonly Mock<IGetPatientProfile> _mockGetPatientProfile;
        private readonly Mock<IGetUploadedPrescriptionsByPatientMobileNumber> _mockGetUploadedPrescriptionsByPatientMobileNumber;
        private readonly Mock<IGetPatientsCreatedPrescriptionCount> _mockGetPatientsCreatedPrescriptionCount;
        private readonly Mock<IGetPatientByDoctorMobile> _mockGetPatientByDoctorMobile;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly PatientService _patientService;
        private readonly MockPatientData _mockPatientData;
        private readonly MockPatientControllerData _mockPatientControllerData;
        
        public PatientServiceTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockGetPatientProfile = new Mock<IGetPatientProfile>();
            _mockPatientDiseaseRepository = new Mock<IPatientDiseaseRepository>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockOneAuthorityService = new Mock<IOneAuthorityService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockPharmacyRepository = new Mock<IPharmacyRepository>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockGetPatientByDoctorMobile = new Mock<IGetPatientByDoctorMobile>();
            _mockFileService = new Mock<IFileService>();
            _mockGetUploadedPrescriptionsByPatientMobileNumber = new Mock<IGetUploadedPrescriptionsByPatientMobileNumber>();
            _mockGetPatientsCreatedPrescriptionCount = new Mock<IGetPatientsCreatedPrescriptionCount>();
            _mockPatientUpdateService = new Mock<IPatientUpdateService>();
            _mockEmailService = new Mock<IEmailService>();
            _patientService = new PatientService(_mockPatientRepository.Object, _mockPatientDiseaseRepository.Object, _mockUserValidations.Object, _mockOneAuthorityService.Object,
                                                 _mockFileService.Object, _mockGetPatientProfile.Object, _mockGetUploadedPrescriptionsByPatientMobileNumber.Object,
                                                 _patientUpdateService, _mockGetPatientsCreatedPrescriptionCount.Object, _mockGetPatientByDoctorMobile.Object,
                                                 _mockEmailService.Object);
            _mockPatientData = new MockPatientData();
            _patientUpdateService = new PatientUpdateService(_mockPatientRepository.Object, _mockFileUpload.Object, _mockFileValidations.Object, _mockPharmacyRepository.Object);
            _mockPatientControllerData = new MockPatientControllerData();
        }

        [Theory, MemberData(nameof(CreatePatientTestInlineData))]
        public async Task Create_PatientObjectIsPassed_ReturnsResponseObjectAsync(bool isUserEmailAlreadyExists, bool isUserMobileNoAlreadyExists)
        {
            var patientDto = _mockPatientData.GetFakePatientDto();
            _mockUserValidations.Setup(x => x.IsEmailAlreadyExists(patientDto.Email)).Returns(isUserEmailAlreadyExists);
            _mockUserValidations.Setup(x => x.IsMobileNumberAlreadyExists(patientDto.Email)).Returns(isUserMobileNoAlreadyExists);
            _mockPatientRepository.Setup(x => x.CreatePatient(_mockPatientData.GetMockPatient())).Returns(It.IsAny<Patient>());
            _mockPatientRepository.Setup(x => x.UpdatePatient(_mockPatientData.GetMockPatient())).Returns(It.IsAny<Patient>());
            _mockPatientDiseaseRepository.Setup(x => x.CreatePatientDisease(_mockPatientData.GetMockPatientDisease())).Returns(It.IsAny<PatientDisease>());
            await _patientService.PatientRegistration(patientDto);
            _mockPatientService.Verify();
        }

        public static IEnumerable<object[]> CreatePatientTestInlineData =>
              new List<object[]>
              {
                    new object[] {false, true },
                    new object[] {true, false},
                    new object[] {true, true }
              };

        /// <summary>
        /// Add patient disease method unit test.
        /// </summary>
        [Theory, MemberData(nameof(AddPatientDiseaseTestInlineData))]
        public async Task AddPatientDisease_WhenCalled_ReturnsResponseDto(ResponseDto expectedResult, bool isSoftDeleted, bool isAdded)
        {
            //Arrange
            _mockPatientDiseaseRepository.Setup(x => x.ReadByIdSoftDeleted(It.IsAny<int>(), It.IsAny<int>())).Returns(isSoftDeleted ? _mockPatientData.Get_PatientDisease_SoftDeleted() : _mockPatientData.Get_NullPatientDisease_SoftDeleted());
            _mockPatientDiseaseRepository.Setup(x => x.Update(It.IsAny<PatientDisease>())).Returns(_mockPatientData.Get_PatientDisease_SoftDeleted());
            _mockPatientDiseaseRepository.Setup(x => x.CreatePatientDisease(It.IsAny<PatientDisease>())).Returns(isAdded ? _mockPatientData.GetMockPatientDisease() : _mockPatientData.Get_NullPatientDisease());

            //Act
            var result = await _patientService.AddPatientDisease(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.True(result.StatusCode == expectedResult.StatusCode);
        }

        //Complex object to pass as parameter in AddPatientDisease_WhenCalled_ReturnsResponseDto for mock response
        public static IEnumerable<object[]> AddPatientDiseaseTestInlineData =>
        new List<object[]>
        {
            new object[] { new ResponseDto { StatusCode = 400 }, false, false },
            new object[] { new ResponseDto { StatusCode = 200 }, false, true },
            new object[] { new ResponseDto { StatusCode = 200 }, true, true },
        };

        /// <summary>
        /// Add patient disease method unit test.
        /// </summary>
        [Theory, MemberData(nameof(RemovePatientDiseaseTestInlineData))]
        public async Task RemovePatientDisease_WhenCalled_ReturnsResponseDto(ResponseDto expectedResult, bool isSoftDeleted)
        {
            //Arrange
            _mockPatientDiseaseRepository.Setup(x => x.ReadById(It.IsAny<int>(), It.IsAny<int>())).Returns(isSoftDeleted ? _mockPatientData.Get_PatientDisease_SoftDeleted() : _mockPatientData.Get_NullPatientDisease_SoftDeleted());
            _mockPatientDiseaseRepository.Setup(x => x.Update(It.IsAny<PatientDisease>())).Returns(_mockPatientData.Get_NullPatientDisease_SoftDeleted());

            //Act
            var result = await _patientService.RemovePatientDisease(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            //Assert
            Assert.True(result.StatusCode == expectedResult.StatusCode);
        }

        //Complex object to pass as parameter in AddPatientDisease_WhenCalled_ReturnsResponseDto for mock response
        public static IEnumerable<object[]> RemovePatientDiseaseTestInlineData =>
        new List<object[]>
        {
            new object[] { new ResponseDto { StatusCode = 400 }, false },
            new object[] { new ResponseDto { StatusCode = 200 }, true },
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Get_PatientByPatientMobileNumber_ReturnsResponseAsync(bool isValidpatientmobileno)
        {
            var mobileno = "7485961263";
            var patient = _mockPatientData.GetFakePatientEntityDto();
            //Arrange
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(mobileno)).Returns(isValidpatientmobileno);
            _mockPatientRepository.Setup(x => x.PatientByMobileNumberAsync(mobileno)).Returns(patient);
            //Act
            await _patientService.GetPatientByMobileNo(mobileno);
            //Assert
            _mockPatientService.Verify();
        }

        /// <summary>
        /// Unit Testing for GET API of Get Patients By Doctor Mobile Number
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="patientName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("9874521365", 0, "test")]
        public async Task To_Check_GetPatients_ByDoctorMobileNumber(string mobileNo, int pageIndex, string patientName)
        {
            //Arrange
            var doctorId = 1;
            _mockPatientRepository.Setup(x => x.GetDoctorIdByDoctoMobileAsync(mobileNo)).Returns(_mockPatientData.GetMockDoctorId);
            _mockPatientRepository.Setup(x => x.GetPrescriptionsByDoctorIdAsync(doctorId)).Returns(_mockPatientData.GetMockPrescriptionList);
            //Act
            var result = await _patientService.GetPatientsByDoctorMobileAsync(mobileNo, pageIndex, patientName);
            //Assert
            _mockPatientService.Verify();
        }

        /// <summary>
        /// Author : Pooja Desai
        /// Unit Testing for GET API of Get Patients By Doctor Mobile Number to check for empty list
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="patientName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("9874521365", 0, "test")]
        public async Task To_Check_GetEmptyPatientList_ByDoctorMobileNumber(string mobileNo, int pageIndex, string patientName)
        {
            var doctorId = 1;
            _mockPatientRepository.Setup(x => x.GetDoctorIdByDoctoMobileAsync(mobileNo)).Returns(_mockPatientData.GetMockDoctorId);
            _mockPatientRepository.Setup(x => x.GetPrescriptionsByDoctorIdAsync(doctorId)).Returns(_mockPatientData.GetMockEmptyPrescriptionList);
            //Act
            var result = await _patientService.GetPatientsByDoctorMobileAsync(mobileNo, pageIndex, patientName);
            //Assert
            _mockPatientService.Verify();
        }

        /// <summary>
        /// Author : Shivam Pandey
        /// Unit Testing for Update Patent profile 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="isPatientEmpty"></param>
        /// <param name="isPatientUpdated"></param>
        /// <returns></returns>
        [Theory]
        [MemberData(nameof(UpdatePatientAsyncTestData))]
        public async void ToCheck_UpdatepatientAsync_PassedSuccessfully(string mobileNumber, bool isPatientEmpty, bool isPatientUpdated)
        {
            _mockPatientRepository.Setup(x => x.GetPatientAsync(mobileNumber)).Returns(isPatientEmpty ? _mockPatientData.Mock_EmptyPatientAsync() : _mockPatientData.Mock_PatientAsync());
            _mockFileValidations.Setup(x => x.ValidateFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>);
            _mockFileValidations.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>);
            _mockFileUpload.Setup(x => x.DeleteFile(_mockPatientData.Mock_Patient().ProfilePictureFilePath)).Returns(true);
            _mockPatientRepository.Setup(x => x.UpdatePatientProfileAsync(_mockPatientData.Mock_Patient())).Returns(isPatientUpdated ? _mockPatientData.Mock_TrueAsync() : _mockPatientData.Mock_FalseAsync());
            await _patientUpdateService.UpdatePatientAsync(mobileNumber, _mockPatientData.Mock_PatientUpdateDto());
            _mockPatientUpdateService.Verify();
        }

        public static IEnumerable<object[]> UpdatePatientAsyncTestData => new List<object[]>
        {
            new object[] { "7845962154", true, true},
            new object[] { "7845962154", false, false},
            new object[] { "7845962154", false, true},
        };

        [Theory]
        [InlineData("9874521365", false)]
        [InlineData("9874521365", true)]
        public async void ToCheck_GetPatientProfileByPatientMobileNumberAsync_Responses(string mobileNumber, bool isMobileValid)
        {
            _mockGetPatientProfile.Setup(x => x.GetPatientProfileByPatientMobileNumberAsync(mobileNumber)).Returns(isMobileValid ? _mockPatientControllerData.GetPatientProfile_ByPatientMobileNo_SuccessResponse() : _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            await _patientService.GetPatientProfileAsync(mobileNumber);
            _mockPatientService.Verify();
        }

        [Theory]
        [InlineData("9874521365", 0, false, false)]
        [InlineData("9874521365", 0, true, false)]
        [InlineData("9874521365", 0, true, true)]
        public async void ToCheck_GetPatientUploadedPrescriptionsByPatientMobileNumberAsync_Responses(string mobileNumber, int pageIndex, bool isMobileValid, bool isPageIndexValid)
        {
            _mockGetUploadedPrescriptionsByPatientMobileNumber.Setup(x => x.GetUploadedPrescriptionByPatientMobileNumberAsync(mobileNumber, pageIndex)).Returns(isMobileValid ? (isPageIndexValid ? _mockPatientControllerData.GetPatientUploadedPrescriptionSuccessResponse() :
                                                                                                                                                                                                    _mockPatientControllerData.GetPatient_InvalidPageIndex_Response()) :
                                                                                                                                                                                                    _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            await _patientService.GetPatientProfileAsync(mobileNumber);
            _mockPatientService.Verify();
        }

        [Theory]
        [InlineData("9874521365", false)]
        [InlineData("9874521365", true)]
        public async void ToCheck_GetPatientsCreatedPrescriptionCountAsync_Responses(string mobileNumber, bool isMobileValid)
        {
            _mockGetPatientsCreatedPrescriptionCount.Setup(x => x.GetPatientsCreatedPrescriptionCountAsync(mobileNumber)).Returns(isMobileValid ? _mockPatientControllerData.GetCreatedPrescriptionsCountDtoResponse() :
                                                                                                                                                  _mockPatientControllerData.GetPatient_InvalidMobileNumber_Response());
            await _patientService.GetPatientProfileAsync(mobileNumber);
            _mockPatientService.Verify();
        }

        /// <summary>
        /// Get List of Prescriptions Status Send For Approval by Patient Mobile Number 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetPrescriptionsStatusSentForApprovalAsyncTestInlineData))]
        public async Task GetPrescriptionStatusSentForApprovalAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber,bool isValid)
        {
            _mockPatientRepository.Setup(x => x.GetPrescriptionStatusByPatientMobileNumberAsync(patientMobileNumber)).Returns(_mockPatientData.GetMockPrescriptionList());
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(patientMobileNumber)).Returns(isValid);

            var result = await _patientService.GetPrescriptionStatusByPatientMobileNumberAsync(pageIndex, patientMobileNumber);
            _mockPatientService.Verify();
        }

        public static IEnumerable<object[]> GetPrescriptionsStatusSentForApprovalAsyncTestInlineData =>
        new List<object[]>
        {
            new object[] { -1, "9876543210",true }, //When pageIndex is invalid
            new object[] { 0, "0123456789",false}, //When mobile number does not exist.
            new object[] { 0, "1023456789",true }, //Success with records
            new object[] { 1, "0123456789",false } //Success with empty list
        };

        /// <summary>
        ///  Get List of Uploaded Prescriptions By Pharmacy by Patient Mobile Number 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <param name="expectedResult"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetUploadedPrescriptionsByPharmacyAsyncTestInlineData))]
        public async Task GetUploadedPrescriptionByPharmacyAsync_WhenCalled_ReturnsResponseDtoAsync(int pageIndex, string patientMobileNumber, bool isValid)
        {
            _mockPatientRepository.Setup(x => x.UploadedPrescriptionByPharmacyPatientMobileNumberAsync(patientMobileNumber)).Returns(_mockPatientData.GetMockUploadedePrescriptionListByPharmacy());
            _mockPatientRepository.Setup(x => x.IsValidMobileNo(patientMobileNumber)).Returns(isValid);

            var result = await _patientService.UploadedPrescriptionByPharmacyPatientMobileNumberAsync(pageIndex, patientMobileNumber);
            _mockPatientService.Verify();
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