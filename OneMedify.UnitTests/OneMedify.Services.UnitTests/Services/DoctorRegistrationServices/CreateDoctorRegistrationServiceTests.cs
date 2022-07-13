using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Services.Services.DoctorServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.DoctorRegistrationServices
{
    public class CreateDoctorRegistrationServiceTests
    {
        private readonly Mock<IDoctorRegistrationService> _mockDoctorRegistration;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IOneAuthorityService> _mockOneAuthoriyService;
        private readonly DoctorRegistrationService _mockDoctorRegistrationService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly MockDoctorData _mockDoctorData;

        public CreateDoctorRegistrationServiceTests()
        {
            _mockDoctorRegistration = new Mock<IDoctorRegistrationService>();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockOneAuthoriyService = new Mock<IOneAuthorityService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockDoctorRegistrationService = new DoctorRegistrationService(_mockDoctorRepository.Object
                                                                           , _mockUserValidations.Object
                                                                           , _mockFileValidations.Object
                                                                           , _mockOneAuthoriyService.Object
                                                                           , _mockFileUpload.Object
                                                                           , _mockEmailService.Object);
            _mockDoctorData = new MockDoctorData();
        }

        [Theory, MemberData(nameof(CreateDoctorRegistrationTestInlineData))]
        public async Task CreateDoctorRegistrationAsync_ReturnsResponseDtoAsync(bool isEmailAlreadyExist, bool isEmailValid, bool isMobileNoValid,bool isDoctorRegistered, bool isDoctorUpdated)
        {
            var doctorDto = _mockDoctorData.ToCheck_FakeDoctorDto();
            _mockUserValidations.Setup(x => x.IsEmailAlreadyExists(doctorDto.Email)).Returns(isEmailAlreadyExist);
            _mockUserValidations.Setup(x => x.IsEmailValid(doctorDto.Email)).Returns(isEmailValid);
            _mockUserValidations.Setup(x => x.IsMobileNumberAlreadyExists(doctorDto.MobileNumber)).Returns(isMobileNoValid);
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>())).Returns(_mockDoctorData.Mock_FileValidationDto().FirstFileExtension);
            _mockFileUpload.Setup(x => x.GetFileName(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockDoctorData.Mock_FileValidationDto().FirstFileName);
            _mockFileUpload.Setup(x => x.GetFilePath(It.IsAny<string>())).Returns(_mockDoctorData.Mock_FileValidationDto().FirstFilePath);
            _mockFileUpload.Setup(x => x.GetLengthOfFile(It.IsAny<string>())).Returns(_mockDoctorData.Mock_FileValidationDto().FirstFileLength);
            _mockFileUpload.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>()));
            _mockDoctorRepository.Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>())).Returns(isDoctorRegistered ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            _mockDoctorRepository.Setup(x => x.UpdateDoctorAsync(It.IsAny<Doctor>())).Returns(isDoctorUpdated ? _mockDoctorData.Mock_TrueAsync() : _mockDoctorData.Mock_FalseAsync());
            _mockOneAuthoriyService.Setup(x => x.RegisterUser(_mockDoctorData.GetFakeUserRegisterModel())).Returns(It.IsAny<Task<ResponseDto>>());
            await _mockDoctorRegistrationService.DoctorRegistrationAsync(doctorDto);
            _mockDoctorRegistration.Verify();
        }

        //Complex object to pass as parameter in CreateDoctorRegistration_ReturnsResponseDto for mock response
        public static IEnumerable<object[]> CreateDoctorRegistrationTestInlineData =>
        new List<object[]>
        {
            new object[] {false, true, true, true, true },
            new object[] {true, false, true, true, true },
            new object[] { true, true, false, true, true },
            new object[] { true, true, true, false, true },
            new object[] { true, true, true, true, false },
            new object[] { false, false, false, false, false},
            new object[] { true, true, true, true, true },

        };
    }
}
