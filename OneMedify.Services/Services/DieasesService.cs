using OneMedify.DTO.Disease;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using System.Collections.Generic;

namespace OneMedify.Services.Services
{
    public class DieasesService : IDieasesService
    {
        private readonly IDiseaseRepository _dieasesRepository;

        public DieasesService(IDiseaseRepository dieasesRepository)
        {
            _dieasesRepository = dieasesRepository;
        }

        /// <summary>
        /// Call DieasesList From DieasesRepository 
        /// </summary>
        public virtual DiseaseResponseDto GetDiseases()
        {
            List<DieasesDto> dieasesList = new List<DieasesDto>();
            try
            {
                var disease = _dieasesRepository.GetDiseases();
                if (disease.Count > 0)
                {
                    foreach (var dieasesItem in disease)
                    {
                        DieasesDto dieasesDto = new DieasesDto
                        {
                            Id = dieasesItem.DiseaseId,
                            Name = dieasesItem.DiseaseName
                        };
                        dieasesList.Add(dieasesDto);
                    }
                }
                return new DiseaseResponseDto { StatusCode = 200, Response = dieasesList };
            }
            catch
            {
                return new DiseaseResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
    }
}
