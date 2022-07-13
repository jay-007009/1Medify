using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;

namespace OneMedify.Infrastructure.Repositories
{
    public class UploadPrescriptionRepository : IUploadPrescriptionRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public UploadPrescriptionRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }
        public PrescriptionUpload Create(PrescriptionUpload prescriptionUpload)
        {
            try
            {
                _oneMedifyDbContext.PrescriptionUploads.Add(prescriptionUpload);
                _oneMedifyDbContext.SaveChanges();
                return prescriptionUpload;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
