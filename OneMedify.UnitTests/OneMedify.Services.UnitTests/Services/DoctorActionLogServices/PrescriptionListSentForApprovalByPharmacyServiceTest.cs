using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Services.Services.DoctorActionLogServices;
using OneMedify.Shared.Contracts;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services.DoctorActionLogServices
{
    public class PrescriptionListSentForApprovalByPharmacyServiceTest
    {
        private readonly Mock<IDoctorActionLogRepository> _mockDoctorActionLogRepository;
        private readonly PrescriptionListSentForApprovalByPharmacy _prescriptionListSentForApprovalByPharmacy;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IPrescriptionListSentForApprovalByPharmacy> _mockPrescriptionListSentForApprovalByPharmacy;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly MockDoctorActionLogData _mockDoctorActionLogData;

        public PrescriptionListSentForApprovalByPharmacyServiceTest()
        {
            _mockDoctorActionLogRepository = new Mock<IDoctorActionLogRepository>(); 
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockFileService = new Mock<IFileService>();
            _mockDoctorActionLogData = new MockDoctorActionLogData();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockPrescriptionListSentForApprovalByPharmacy = new Mock<IPrescriptionListSentForApprovalByPharmacy>();
            _prescriptionListSentForApprovalByPharmacy = new PrescriptionListSentForApprovalByPharmacy(_mockDoctorActionLogRepository.Object,
                                                                                                   _mockFileService.Object, _mockDoctorRepository.Object, _mockPrescriptionRepository.Object);
        }

        /// <summary>
        /// Prescription List By Pharmacy Using Doctor Mobile Number ServiceTest
        /// </summary>
        /// <param name="doctorMobile"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isDoctorMobileValid"></param>
        [Theory]
        [InlineData("1234567890",true)]
        [InlineData("1234567890", false)]
        public async void ToCheck_PrescriptionListByPharmacy(string doctorMobile, bool isDoctorMobileValid)
        {

            _mockDoctorRepository.Setup(x => x.IsValidMobileNumber(It.IsAny<string>())).Returns(isDoctorMobileValid);
            _mockDoctorRepository.Setup(x => x.ReadDoctorByMobileNumber(It.IsAny<string>())).Returns(It.IsAny<Task<Doctor>>());
            _mockPrescriptionRepository.Setup(x => x.ReadById(It.IsAny<int>())).Returns(It.IsAny<Task<Prescription>>());
            _mockDoctorActionLogRepository.Setup(x => x.GetPendingPrescriptions()
                                          ).Returns(_mockDoctorActionLogData.Get_PrescriptionList_SentForApprovalbyPharmacy_Response());
            _mockPrescriptionRepository.Setup(x => x.GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPharmacy(doctorMobile)).Returns(Task.FromResult(It.IsAny<List<Prescription>>()));
            await _prescriptionListSentForApprovalByPharmacy.GetPrescriptionListSentForApprovalByPharmacy(doctorMobile);
            _mockPrescriptionListSentForApprovalByPharmacy.Verify();
        }
    }
}
