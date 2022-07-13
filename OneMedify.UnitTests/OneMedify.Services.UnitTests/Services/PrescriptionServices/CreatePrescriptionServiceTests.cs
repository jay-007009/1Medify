using Moq;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Services.PrescriptionServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.PrescriptionServices
{
    public class CreatePrescriptionServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<IMedicineRepository> _mockMedicineRepository;
        private readonly Mock<IDiseaseRepository> _mockDieasesRepository;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly Mock<IPrescriptionMedicineRepository> _mockPrescriptionMedicineRepository;
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly MockPrescriptionData _mockPrescriptionData;
        private readonly CreatePrescriptionService _createPrescriptionService;
        public CreatePrescriptionServiceTests()
        {
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockMedicineRepository = new Mock<IMedicineRepository>();
            _mockDieasesRepository = new Mock<IDiseaseRepository>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockPrescriptionMedicineRepository = new Mock<IPrescriptionMedicineRepository>();
            _mockPatientService = new Mock<IPatientService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockPrescriptionData = new MockPrescriptionData();
            _createPrescriptionService = new CreatePrescriptionService(_mockDoctorRepository.Object, _mockPatientRepository.Object,
                                                                        _mockMedicineRepository.Object, _mockDieasesRepository.Object,
                                                                        _mockPrescriptionRepository.Object, _mockPrescriptionMedicineRepository.Object,
                                                                        _mockPatientService.Object, _mockEmailService.Object);
        }

        /// <summary>
        /// Create prescription service unit test
        /// </summary>
        [Theory, MemberData(nameof(CreatePrescriptionTestInlineData))]
        public async Task CreatePrescriptionAsync_ReturnsResponseDtoAsync(ResponseDto expectedResponseDto, bool isDoctor,
                                                                            bool isPatient, bool isDisease, bool isMedicine,
                                                                            bool isPrescription)
        {
            //Arrange
            var mockprescriptionData = _mockPrescriptionData.Get_PrescriptionDto_Response();
            _mockDoctorRepository.Setup(x => x.ReadDoctorByMobileNumber(It.IsAny<string>())).Returns(isDoctor ? _mockPrescriptionData.Get_Doctor_Response() : _mockPrescriptionData.Get_NullDoctor_Response());
            _mockPatientRepository.Setup(x => x.ReadPatientByMobileNumber(It.IsAny<string>())).Returns(isPatient ? _mockPrescriptionData.Get_Patient_Response() : _mockPrescriptionData.Get_NullPatient_Response());
            _mockDieasesRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(isDisease ? _mockPrescriptionData.Get_Disease_Response() : _mockPrescriptionData.Get_NullDisease_Response());
            _mockMedicineRepository.Setup(x => x.GetMedicineByDiseaseId(It.IsAny<int>())).Returns(_mockPrescriptionData.Get_MedicineList_Response());
            _mockMedicineRepository.Setup(x => x.GetMedicineByDiseaseId(It.IsAny<int>(), It.IsAny<int>())).Returns(isMedicine ? _mockPrescriptionData.Get_Medicine_Response() : _mockPrescriptionData.Get_NullMedicine_Response());
            _mockPrescriptionRepository.Setup(x => x.Create(It.IsAny<Prescription>())).Returns(isPrescription ? _mockPrescriptionData.Get_Prescription_Response() : _mockPrescriptionData.Get_NullPrescription_Response());
            _mockPatientService.Setup(x => x.AddPatientDisease(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(It.IsAny<Task<ResponseDto>>());
            _mockPatientService.Setup(x => x.RemovePatientDisease(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(It.IsAny<Task<ResponseDto>>());
            _mockPrescriptionMedicineRepository.Setup(x => x.Create(It.IsAny<PrescriptionMedicine>())).Returns(_mockPrescriptionData.Get_PrescriptionMedicine_Response());
            _mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(It.IsAny<ResponseDto>()));

            //Act
            var result = await _createPrescriptionService.CreatePrescription(mockprescriptionData);

            //Assert
            Assert.True(result.StatusCode == expectedResponseDto.StatusCode);
        }

        //Complex object to pass as parameter in CreatePrescription_ReturnsResponseDto for mock response
        public static IEnumerable<object[]> CreatePrescriptionTestInlineData =>
        new List<object[]>
        {
            new object[] { new ResponseDto { StatusCode = 400 }, false, true, true, true, true },
            new object[] { new ResponseDto { StatusCode = 400 }, true, false, true, true, true },
            new object[] { new ResponseDto { StatusCode = 400 }, true, true, false, true, true },
            new object[] { new ResponseDto { StatusCode = 400 }, true, true, true, false, true },
            new object[] { new ResponseDto { StatusCode = 400 }, true, true, true, true, false },
            new object[] { new ResponseDto { StatusCode = 200 }, true, true, true, true, true },

        };
    }
}
