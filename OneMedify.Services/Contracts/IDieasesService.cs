using OneMedify.DTO.Disease;

namespace OneMedify.Services.Contracts
{
    public interface IDieasesService
    {
        /// <summary>
        /// Call DieasesList From DieasesRepository 
        /// </summary>
        DiseaseResponseDto GetDiseases();
    }
}
