using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneMedify.Infrastructure.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public StateRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Get All State
        /// </summary>
        public virtual List<State> GetStates()
        {
            try
            {
                var states = _oneMedifyDbContext.States.OrderBy(state => state.StateName).ToList();
                return states;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
