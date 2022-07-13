using Microsoft.AspNetCore.Mvc;
using OneMedify.Services.Contracts;

namespace OneMedify.API.Controllers
{
    [Route("api/Address/[Controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _state;

        public StateController(IStateService state)
        {
            _state = state;
        }

        [HttpGet]
        public IActionResult GetStates()
        {
            var states = _state.GetStates();
            return StatusCode(states.StatusCode, states);
        }
    }
}