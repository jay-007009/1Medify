using OneMedify.DTO.Response;

namespace OneMedify.Services.Contracts
{
    public interface ICityService
    {
        ResponseDto GetCitiesByStateId(int? stateId);
    }
}
