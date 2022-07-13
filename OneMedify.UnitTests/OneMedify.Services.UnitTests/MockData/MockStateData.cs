using OneMedify.Infrastructure.Entities;
using System.Collections.Generic;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData
{
    public class MockStateData
    {
        public List<State> GetMockListOfAllState()
        {
            List<State> states = new List<State>()
            {
                new State
                {
                   StateId = 1,
                   StateName = "Gujarat"
                }
            };
            return states;
        }
        public List<State> GetMockListOfEmptyState()
        {
            List<State> states = new List<State>()
            {
                new State{}
            };
            return states;
        }
    }
}
