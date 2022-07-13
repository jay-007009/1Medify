using Moq;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Services.Contracts;
using OneMedify.Services.Services;
using OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData;
using System.Collections.Generic;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.Services
{
    public class StateServiceTest
    {
        private readonly Mock<IStateService> _mockStateService;
        private readonly Mock<IStateRepository> _mockStateRepository;
        private readonly MockStateData _mockStateData;
        private readonly StateService _stateService;

        public StateServiceTest()
        {
            _mockStateRepository = new Mock<IStateRepository>();
            _mockStateData = new MockStateData();
            _stateService = new StateService(_mockStateRepository.Object);
            _mockStateService = new Mock<IStateService>();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for StateService for getting all states list 
        /// </summary>
        [Fact]
        public void To_Check_Get_All_State()
        {
            //Arrange
            var state = _mockStateRepository.Setup(x => x.GetStates()).Returns(_mockStateData.GetMockListOfAllState());
            //Act
            var result = _stateService.GetStates();
            //Assert
            _mockStateService.Verify();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for StateService for getting empty state list 
        /// </summary>
        [Fact]
        public void To_Check_Get_Empty_State()
        {
            //Arrange
            var state = _mockStateRepository.Setup(x => x.GetStates()).Returns(new List<State>());
            //Act
            var result = _stateService.GetStates();
            //Assert
            _mockStateService.Verify();
        }
    }
}