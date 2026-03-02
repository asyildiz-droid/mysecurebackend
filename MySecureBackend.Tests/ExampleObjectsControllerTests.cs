using Microsoft.AspNetCore.Mvc;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.Tests
{
    [TestClass]
    public sealed class ExampleObjectsControllerTests
    {
        private Environment2DController controller;
        private Mock<IEnvironment2DRepository> exampleObjectRepository;
        private Mock<IAuthenticationService> authenticationService;

        [TestInitialize]
        public void Setup()
        {
            exampleObjectRepository = new Mock<IEnvironment2DRepository>();
            authenticationService = new Mock<IAuthenticationService>();

            controller = new Environment2DController(exampleObjectRepository.Object, authenticationService.Object);
        }

        [TestMethod]
        public async Task Get_ExampleObjectThatDoesNotExist_Returns404NotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            exampleObjectRepository.Setup(x => x.SelectAsync(id)).ReturnsAsync(null as Environment2D);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(response.Result);
        }
    }
}