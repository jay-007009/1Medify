using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Services.Services.PharmacyServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PharmacyServices
{
    public class PharmacyRegistrationServiceTests
    {
        private readonly Mock<IPharmacyRegistrationService> _mockPharmacyRegistrationService;
        private readonly Mock<IPharmacyRepository> _mockPharmacyRepository;
        private readonly Mock<IFileUpload> _mockFileUpload;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IOneAuthorityService> _mockOneAuthoriyService;
        private readonly Mock<IFileValidations> _mockFileValidations;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly PharmacyRegistrationService _pharmacyRegistrationService;
        private readonly MockPharmacyData _mockPharmacyData;
        

        public PharmacyRegistrationServiceTests()
        {
            _mockPharmacyRegistrationService = new Mock<IPharmacyRegistrationService>();
            _mockPharmacyRepository = new Mock<IPharmacyRepository>();
            _mockFileUpload = new Mock<IFileUpload>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockOneAuthoriyService = new Mock<IOneAuthorityService>();
            _mockFileValidations = new Mock<IFileValidations>();
            _mockEmailService = new Mock<IEmailService>();
            _pharmacyRegistrationService = new PharmacyRegistrationService(_mockPharmacyRepository.Object, _mockFileUpload.Object, _mockUserValidations.Object, _mockOneAuthoriyService.Object, _mockFileValidations.Object,_mockEmailService.Object);
            _mockPharmacyData = new MockPharmacyData();
        }

        [Theory]
        [MemberData(nameof(PharmacyRegistrationTestInlineData))]
        public async void ToCheck_PharmacyRegistrationAsync_ResponsesAsync(bool isEmailAlreadyExists, bool isEmailValid, bool isMobileValid, bool isCityVerified, bool isPharmacyRegistered, bool isPharmacyUpdated)
        {
            var pharmacyDto = _mockPharmacyData.GetFakePharmacySignUpDto();
            var pharmacy = _mockPharmacyData.GetFakePharmacy();
            var user = _mockPharmacyData.GetFakeUserRegisterModel();
            _mockUserValidations.Setup(pharamcy => pharamcy.IsEmailAlreadyExists(pharmacyDto.Email)).Returns(isEmailAlreadyExists);
            _mockUserValidations.Setup(pharamcy => pharamcy.IsEmailValid(pharmacyDto.Email)).Returns(isEmailValid);
            _mockUserValidations.Setup(pharamcy => pharamcy.IsMobileNumberAlreadyExists(pharmacyDto.MobileNumber)).Returns(isMobileValid);
            _mockPharmacyRepository.Setup(pharamcy => pharamcy.VerfifyCity(pharmacyDto.CityId)).Returns(isCityVerified);
            _mockOneAuthoriyService.Setup(x => x.RegisterUser(_mockPharmacyData.GetFakeUserRegisterModel())).Returns(It.IsAny<Task<ResponseDto>>());
            _mockPharmacyRepository.Setup(x => x.VerfifyCity(pharmacyDto.CityId)).Returns(isCityVerified);
            _mockFileUpload.Setup(x => x.GetExtensionOfDocument(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileUpload.Setup(x => x.GetFileName(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileUpload.Setup(x => x.GetFilePath(It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileUpload.Setup(x => x.GetLengthOfFile(It.IsAny<string>())).Returns(It.IsAny<int>());
            _mockFileUpload.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>()));
            _mockPharmacyRepository.Setup(x => x.PharmacyRegistration(pharmacy)).Returns(isPharmacyRegistered);
            _mockPharmacyRepository.Setup(x => x.UpdatePharmacy(pharmacy)).Returns(isPharmacyUpdated);
            await _pharmacyRegistrationService.PharmacyRegistrationAsync(pharmacyDto);
            _mockPharmacyRegistrationService.Verify();
        }

        public static IEnumerable<object[]> PharmacyRegistrationTestInlineData =>
            new List<object[]>
            {
                new object[] { true, false, false, false, true, true },
                new object[] { false, true, false, false, true, true },
                new object[] { false, false, true, false, true, true },
                new object[] { false, false, false, true, true, true },
                new object[] { false, false, false, false, false, true },
                new object[] { false, false, false, false, true, false },
                new object[] { false, false, false, false, true, true },
            };
    }
}