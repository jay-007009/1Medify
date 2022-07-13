using OneMedify.Infrastructure.Entities;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IUploadPrescriptionRepository
    {
        PrescriptionUpload Create(PrescriptionUpload prescriptionUpload);
    }
}
