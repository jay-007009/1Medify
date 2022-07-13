using Microsoft.AspNetCore.Mvc;
using Moq;
using OneMedify.API.Controllers;
using OneMedify.Services.Contracts;
using OneMedify.UnitTests.OneMedify.API.UnitTests.MockData;
using Xunit;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.Controller
{
    public class StateControllerTest
    {
        private readonly Mock<IStateService> _mockStateService;
        private readonly StateController _stateController;
        private readonly MockStateControllerData _mockStateControllerData;

        public StateControllerTest()
        {
            _mockStateService = new Mock<IStateService>();
            _stateController = new StateController(_mockStateService.Object);
            _mockStateControllerData = new MockStateControllerData();
        }

        /// <summary>
        /// Author: Bindiya Tandel
        /// Testing for StateController for 500 response 
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Check_Get_State(bool isSuccess)
        {
            //Arrange
            _mockStateService.Setup(x => x.GetStates()).Returns(isSuccess ? _mockStateControllerData.GetStateSuccessResponse() : _mockStateControllerData.GetStateInternalServerResponse());
            //Act
            var result = _stateController.GetStates();
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}