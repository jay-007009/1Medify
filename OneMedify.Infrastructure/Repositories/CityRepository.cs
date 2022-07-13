using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneMedify.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public CityRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// GetAllCitiesByStateId
        /// </summary>
        public List<City> GetCitiesByStateId(int stateId)
        { 
            try
            {
                return _oneMedifyDbContext.Cities.Where(state => state.StateId == stateId && state.IsDisable == false).OrderBy(city=>city.CityName).ToList();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public bool IsValidStateId(int? stateId)
        {
            if (stateId == null)
            {
                return true;
            }
            return false;
        }
    }
}
