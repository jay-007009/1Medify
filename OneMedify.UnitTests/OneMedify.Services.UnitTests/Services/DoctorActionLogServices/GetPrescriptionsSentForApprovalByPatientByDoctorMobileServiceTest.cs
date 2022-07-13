using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Services.Services.DoctorActionLogServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.DoctorActionLogServices
{
    public class GetPrescriptionsSentForApprovalByPatientByDoctorMobileServiceTest
    {
        private readonly Mock<IDoctorActionLogRepository> _mockDoctorActionLogRepository;
        private readonly MockDoctorActionLogData _mockDoctorActionLogData;
        private readonly MockPrescriptionData _mockPrescriptionData;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly GetPrescriptionsSentForApprovalByPatientByDoctorMobileService _prescriptionsSentForApprovalByPatientByDoctorMobileService;
        private readonly Mock<IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService> _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService;

        public GetPrescriptionsSentForApprovalByPatientByDoctorMobileServiceTest()
        {
            _mockDoctorActionLogRepository = new Mock<IDoctorActionLogRepository>();
            _mockDoctorActionLogData = new MockDoctorActionLogData();
            _mockFileService = new Mock<IFileService>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService = new Mock<IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService>();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockPrescriptionData = new MockPrescriptionData();
            _prescriptionsSentForApprovalByPatientByDoctorMobileService = new GetPrescriptionsSentForApprovalByPatientByDoctorMobileService(_mockDoctorActionLogRepository.Object,
                                                                                                                                            _mockFileService.Object, _mockDoctorRepository.Object,
                                                                                                                                            _mockPrescriptionRepository.Object);
        }

        [Theory]
        [InlineData("1234567890", true)]
        [InlineData("1234567890", false)]
        public async void ToCheck_PrescriptionListByPatient(string doctorMobile, bool isDoctorMobileValid)
        {

            _mockDoctorActionLogRepository.Setup(x => x.IsDoctorMobileNumberExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(isDoctorMobileValid));
            _mockDoctorRepository.Setup(x => x.ReadDoctorByMobileNumber(It.IsAny<string>())).Returns(Task.FromResult(new Doctor { DoctorId = 101 }));
            _mockPrescriptionRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(_mockPrescriptionData.Get_Created_Prescription_By_PrescriptionId_Response());
            _mockPrescriptionRepository.Setup(x => x.GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPatient(It.IsAny<string>())).Returns(_mockPrescriptionData.Get_PrescriptionList_Response());
            _mockDoctorActionLogRepository.Setup(x => x.GetPendingPrescriptions())
                                                                     .Returns(_mockDoctorActionLogData.Get_PrescriptionList_SentForApprovalbyPatient_Response());
            await _prescriptionsSentForApprovalByPatientByDoctorMobileService.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctorMobile);
            _mockGetPrescriptionsSentForApprovalByPatientByDoctorMobileService.Verify();
        }    
    }
}
