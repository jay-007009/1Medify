using OneMedify.DTO.Medicine;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<ResponseDto> GetMedicinesByDiseasesIdsAsync(List<int> diseasesId)
        {
            try
            {
                if (_medicineRepository.IsValidDiseasesId(diseasesId))
                {
                    return new ResponseDto { StatusCode = 400, Response = StatusCodeResource.BadRequestResponse };
                }
                foreach (var diseasesIds in diseasesId)
                {
                    var diseaseExist = await _medicineRepository.DiseasesIdExists(diseasesIds);
                    if (diseaseExist == null)
                    {
                        return new ResponseDto { StatusCode = 404, Response = StatusCodeResource.NotFoundResponse };
                    }
                }

                List<MedicineDto> medicineDtos = new List<MedicineDto>();
                var medicines = await _medicineRepository.GetMedicinesByDiseasesIdsAsync(diseasesId);
                foreach (var medicine in medicines)
                {
                    MedicineDto medicineDto = new MedicineDto()
                    {
                        Id = medicine.MedicineId,
                        Name = medicine.MedicineName,
                        DiseasesId = medicine.DiseaseId
                    };
                    medicineDtos.Add(medicineDto);
                }
                return new ResponseDto { StatusCode = 200, Response = medicineDtos };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
    }
}
