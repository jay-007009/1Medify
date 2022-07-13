using OneMedify.DTO.City;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using System.Linq;

namespace OneMedify.Services.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// GetAllCitiesByStateId
        /// </summary>
        public ResponseDto GetCitiesByStateId(int? stateId)
        {
            try
            {
                if (_cityRepository.IsValidStateId(stateId))
                {
                    return new ResponseDto { StatusCode = 400, Response = StatusCodeResource.BadRequestResponse };
                }
                var cities = _cityRepository.GetCitiesByStateId((int)stateId).Select(city => new CityDto
                {
                    Id = city.CityId,
                    Name = city.CityName
                }).ToList();
                return new ResponseDto { StatusCode = 200, Response = cities };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
    }
}