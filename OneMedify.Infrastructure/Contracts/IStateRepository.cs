using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.Infrastructure.Contracts
{
    public interface IStateRepository
    {
        List<State> GetStates();
    }
}