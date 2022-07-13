using OneMedify.DTO.Response;
using OneMedify.DTO.State;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using System.Linq;

namespace OneMedify.Services.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;

        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        /// <summary>
        /// Get All State
        /// </summary>
        public ResponseDto GetStates()
        {
            try
            {
                var states = _stateRepository.GetStates().Select(state => new StateDto
                {
                    Id = state.StateId,
                    Name = state.StateName
                }).ToList();
                return new ResponseDto { StatusCode = 200, Response = states };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
    }
}