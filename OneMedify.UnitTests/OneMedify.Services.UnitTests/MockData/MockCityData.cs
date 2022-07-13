
using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData
{
    public class MockCityData
    {
        public List<City> GetAllCitiesByStateId(int? stateId)
        {
            List<City> cities = new List<City>();
            new City { CityId = 2, CityName = "Gujarat" };
            return cities;
        }

        public List< City> GetMockEmptyCitiesByStateId(int? stateId)
        {
            List<City> cities = new List<City>();
            new City
            {

            };
            return cities;
        }
  
    }
}
