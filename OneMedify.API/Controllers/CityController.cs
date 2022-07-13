using Microsoft.AspNetCore.Mvc;
using OneMedify.Services.Contracts;


namespace OneMedify.API.Controllers
{
    [Route("api/Address/[Controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        /// <summary>
        /// Get Cities by State Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet("{stateId}")]
        public IActionResult Get(int? stateId)
        {
            var cities = _cityService.GetCitiesByStateId(stateId);
            return StatusCode(cities.StatusCode, cities);
        }
    }
}