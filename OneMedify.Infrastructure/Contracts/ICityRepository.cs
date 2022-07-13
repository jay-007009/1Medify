using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.Infrastructure.Contracts
{
    public interface ICityRepository
    {
        List<City> GetCitiesByStateId(int stateId);
        bool IsValidStateId(int? stateId);
    }
}
