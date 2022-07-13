using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Services.Services;
using OneMedify.Services.Services.DoctorServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IDoctorRegistrationService> _mockDoctorRegistrationService;
        private readonly Mock<IDoctorService> _mockDoctorService;
        private readonly Mock<IDoctorUpdateService> _mockDoctorUpdateService;
        private readonly Mock<IGetDoctorByDoctorMobileNoService> _mockGetDoctorByDoctorMobileNoService;
        private readonly Mock<IPatientsPharmacyCountService> _mockPatientsPharmacyCountService;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IOneAuthorityService> _mockoneAuthorityService;
        private readonly DoctorRegistrationService _doctorRegistrationService;
        private readonly DoctorService _doctorService;
        private readonly DoctorUpdateService _doctorUpdateService;
        private readonly PatientsPharmacyCountService _patientsPharmacyCountService;
        private readonly MockDoctorData _mockDoctorData;
        private readonly Mock<IGetDoctorsCountService> _mockGetDoctorsCountService;
        private readonly GetDoctorsCountService _getDoctorsCountService;
        private readonly GetDoctorsListService _getDoctorsListService;
        private readonly Mock<IGetDoctorsListService> _mockGetDoctorsListService;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IEmailService> _mockEmailService;


        public DoctorServiceTests()
        {
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockFileService = new Mock<IFileService>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockDoctorRegistrationService = new Mock<IDoctorRegistrationService>();
            _mockDoctorUpdateService = new Mock<IDoctorUpdateService>();
            _mockPatientsPharmacyCountService = new Mock<IPatientsPharmacyCountService>();
            _mockGetDoctorByDoctorMobileNoService = new Mock<IGetDoctorByDoctorMobileNoService>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockoneAuthorityService = new Mock<IOneAuthorityService>();
            _mockDoctorData = new MockDoctorData();
            _mockGetDoctorsCountService = new Mock<IGetDoctorsCountService>();
            _mockGetDoctorsListService = new Mock<IGetDoctorsListService>();
            _mockDoctorService = new Mock<IDoctorService>();
            _mockEmailService = new Mock<IEmailService>();
            _doctorUpdateService = new DoctorUpdateService(_mockDoctorRepository.Object, _mockFileUpload.Object, _mockFileValidations.Object);
            _doctorRegistrationService = new DoctorRegistrationService(_mockDoctorRepository.Object, _mockUserValidations.Object, _mockFileValidations.Object, _mockoneAuthorityService.Object, _mockFileUpload.Object , _mockEmailService.Object);
            _getDoctorsCountService = new GetDoctorsCountService(_mockDoctorRepository.Object);
            _getDoctorsListService = new GetDoctorsListService(_mockDoctorRepository.Object, _mockFileService.Object);
            _patientsPharmacyCountService = new PatientsPharmacyCountService(_mockPatientRepository.Object, _mockDoctorRepository.Object);
            _doctorService = new DoctorService(_mockDoctorUpdateService.Object, _mockDoctorRegistrationService.Object,
                                               _mockGetDoctorByDoctorMobileNoService.Object, _mockPatientsPharmacyCountService.Object,
                                               _mockGetDoctorsCountService.Object, _mockGetDoctorsListService.Object);
        }

        /// <summary>
        /// Testing of Service Layer
        /// Author: Ketan Singh
        /// </summary>
        /// <param name="isEmailValid"></param>
        /// <param name="isMobileNoValid"></param>
        /// <param name="isDoctorRegistered"></param>
        /// <param name="isDoctorUpdated"></param>
        [Theory]
        [MemberData(nameof(CreateDoctorRegistrationAsync))]
        public async void ToCheck_DoctorRegistrationAsync_PassedSuccessfully(bool isEmailValid, bool isMobileNoValid, bool isDoctorRegistered, bool isDoctorUpdated)
        {
            var doctorDto = _mockDoctorData.ToCheck_FakeDoctorDto();
            _mockUserValidations.Setup(x => x.IsEmailAlreadyExists(doctorDto.Email)).Returns(isEmailValid);
            _mockUserValidations.Setup(x => x.IsMobileNumberAlreadyExists(doctorDto.MobileNumber)).Returns(isMobileNoValid);
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>()));
            _mockFileUpload.Setup(x => x.GetFileName(It.IsAny<string>(), It.IsAny<string>()));
            _mockFileUpload.Setup(x => x.GetFilePath(It.IsAny<string>()));
            _mockFileUpload.Setup(x => x.GetLengthOfFile(It.IsAny<string>())).Returns(It.IsAny<int>());
            _mockFileUpload.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>()));
            _mockDoctorRepository.Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>())).Returns(isDoctorRegistered ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            _mockDoctorRepository.Setup(x => x.UpdateDoctorAsync(It.IsAny<Doctor>())).Returns(isDoctorUpdated ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            _mockoneAuthorityService.Setup(x => x.RegisterUser(_mockDoctorData.GetFakeUserRegisterModel())).Returns(It.IsAny<Task<ResponseDto>>());
            await _doctorRegistrationService.DoctorRegistrationAsync(doctorDto);
            _mockDoctorRegistrationService.Verify();
        }
        public static IEnumerable<object[]> CreateDoctorRegistrationAsync => new List<object[]>
        {
            new object[] { false, true, true, true},
            new object[] { true, false, true, true},
            new object[] { true, true, false, true},
            new object[]{ true, true, true, false},
        };

        [Theory]
        [MemberData(nameof(UpdateDoctorAsyncTestData))]
        public async void ToCheck_UpdateDoctorAsync_PassedSuccessfully(string mobileNumber, bool isDoctorEmpty, bool isDoctorUpdated)
        {
            _mockDoctorRepository.Setup(x => x.GetDoctorAsync(mobileNumber)).Returns(isDoctorEmpty ? _mockDoctorData.Mock_EmptyDoctorAsync() : _mockDoctorData.Mock_DoctorAsync());
            _mockFileValidations.Setup(x => x.ValidateFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>);
            _mockFileValidations.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<ResponseDto>);
            _mockFileUpload.Setup(x => x.DeleteFile(_mockDoctorData.Mock_Doctor().DoctorDegreeFilePath)).Returns(true);
            _mockDoctorRepository.Setup(x => x.UpdateDoctorProfileAsync(_mockDoctorData.Mock_Doctor())).Returns(isDoctorUpdated ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            await _doctorUpdateService.UpdateDoctorAsync(mobileNumber, _mockDoctorData.Mock_DoctorUpdateDto());
            _mockDoctorUpdateService.Verify();
        }

        public static IEnumerable<object[]> UpdateDoctorAsyncTestData => new List<object[]>
        {
            new object[] { "7845962154", true, true},
            new object[] { "7845962154", false, false},
            new object[] { "7845962154", false, true},
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void ToCheck_DoctorbyDoctorMobileNoAsync_PassedSuccessfully(bool isMobileNoValid)
        {
            var mobileno = "1234567890";
            var doctor = _mockDoctorData.GetFakeDoctorEntityDto();
            //Arrange
            _mockDoctorRepository.Setup(x => x.IsValidMobileNumber(mobileno)).Returns(isMobileNoValid);
            _mockDoctorRepository.Setup(x => x.GetDoctorByDoctorMobileNumberAsync(It.IsAny<string>())).Returns(doctor);
            //Act
            await _doctorService.GetDoctorByDoctorMobileNoAsync(mobileno);
            //Assert
            _mockDoctorService.Verify();
        }

        [Theory]
        [InlineData("1234567890", true)]
        [InlineData("1234567890", false)]
        public async void ToCheck_GetPatientPharmacyCountAsync_PassedSuccessfully(string mobileNumber, bool isMobileValid)
        {
            _mockDoctorRepository.Setup(x => x.IsMobileNumberValidAsync(mobileNumber)).Returns(isMobileValid ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            _mockPatientRepository.Setup(x => x.GetDoctorIdByDoctoMobileAsync(mobileNumber)).Returns(_mockDoctorData.Mock_DoctorIdAsync);
            _mockDoctorRepository.Setup(x => x.GetPatientsCountByDoctorIdAsync(It.IsAny<int>())).Returns(_mockDoctorData.Mock_DoctorIdAsync);
            _mockDoctorRepository.Setup(x => x.GetPharmaciesCount()).Returns(_mockDoctorData.Mock_DoctorIdAsync);
            await _patientsPharmacyCountService.GetPatientPharmacyCountAsync(mobileNumber);
            _mockDoctorService.Verify();
        }

        [Theory]
        [InlineData(0, "test")]
        public async void ToCheck_AllDoctorsList(int pageIndex, string doctorName)
        {
            //Arrange
            var doctors = _mockDoctorRepository.Setup(x => x.GetAllDoctorsAsync()).Returns(_mockDoctorData.GetMockDoctorListDto());
            //Act
            var result = await _doctorService.GetAllDoctorsAsync(pageIndex, doctorName);
            //Assert
            _mockDoctorService.VerifyAll();
        }
        [Theory]
        [InlineData(0, "test")]
        public async void ToCheck_EmptyDoctorsList(int pageIndex, string doctorName)
        {
            //Arrange
            var doctors = _mockDoctorRepository.Setup(x => x.GetAllDoctorsAsync()).Returns(_mockDoctorData.GetMockEmptyDoctorList());
            //Act
            var result = await _doctorService.GetAllDoctorsAsync(pageIndex, doctorName);
            //Assert
            _mockDoctorService.VerifyAll();
        }

        [Fact]
        public async void ToCheck_DoctorsCount()
        {
            //Arrange
            var doctorCount = _mockDoctorRepository.Setup(x => x.GetDoctorsCountAsync()).Returns(_mockDoctorData.GetMockDoctorsCount());
            //Act
            var result = await _doctorService.GetDoctorsCountAsync();
            //Assert
            _mockDoctorService.VerifyAll();
        }
    }
}