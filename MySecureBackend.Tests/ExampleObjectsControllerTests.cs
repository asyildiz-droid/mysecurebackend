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
        private Mock<IEnvironment2DRepository> environment2DRepository;
        private Mock<IAuthenticationService> authenticationService;

        [TestInitialize]
        public void Setup()
        {
            environment2DRepository = new Mock<IEnvironment2DRepository>();
            authenticationService = new Mock<IAuthenticationService>();

            controller = new Environment2DController(environment2DRepository.Object, authenticationService.Object);
        }

        #region Bestaande test
        [TestMethod]
        public async Task Get_ExampleObjectThatDoesNotExist_Returns404NotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            environment2DRepository.Setup(x => x.SelectAsync(id)).ReturnsAsync(null as Environment2D);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(response.Result);
        }
        #endregion

        #region AC1 - Gebruiker moet ingelogd zijn

        [TestMethod]
        public async Task AddAsync_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string)null);
            var newEnvironment = new Environment2D { Name = "TestWorld" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<UnauthorizedObjectResult>(response.Result);
            var result = response.Result as UnauthorizedObjectResult;
            var problem = result.Value as ProblemDetails;
            Assert.AreEqual("Gebruiker niet ingelogd", problem.Detail);
        }

        [TestMethod]
        public async Task AddAsync_UserLoggedIn_ContinuesValidation()
        {
            // Arrange
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(new List<Environment2D>());
            environment2DRepository.Setup(x => x.SelectByUserIdAndNameAsync(userId, It.IsAny<string>())).ReturnsAsync(null as Environment2D);
            var newEnvironment = new Environment2D { Name = "TestWorld" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<CreatedAtRouteResult>(response.Result);
        }

        #endregion

        #region AC2 - Maximum van 5 eigen 2D-werelden

        [TestMethod]
        public async Task AddAsync_UserHas4Worlds_CanAddNewWorld()
        {
            // Arrange
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

            var existingWorlds = new List<Environment2D>
            {
                new Environment2D { Id = Guid.NewGuid(), Name = "World1", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World2", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World3", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World4", UserId = userId }
            };

            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(existingWorlds);
            environment2DRepository.Setup(x => x.SelectByUserIdAndNameAsync(userId, It.IsAny<string>())).ReturnsAsync(null as Environment2D);
            var newEnvironment = new Environment2D { Name = "World5" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<CreatedAtRouteResult>(response.Result);
        }

        [TestMethod]
        public async Task AddAsync_UserHas5Worlds_CannotAddNewWorld()
        {
            // Arrange
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

            var existingWorlds = new List<Environment2D>
            {
                new Environment2D { Id = Guid.NewGuid(), Name = "World1", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World2", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World3", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World4", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World5", UserId = userId }
            };

            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(existingWorlds);
            var newEnvironment = new Environment2D { Name = "World6" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(response.Result);
            var result = response.Result as BadRequestObjectResult;
            var problem = result.Value as ProblemDetails;
            Assert.AreEqual("Maximum aantal 2D-werelden bereikt", problem.Detail);
        }

        [TestMethod]
        public async Task AddAsync_UserHasMoreThan5Worlds_CannotAddNewWorld()
        {
            // Arrange - defensief testen
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);

            var existingWorlds = new List<Environment2D>
            {
                new Environment2D { Id = Guid.NewGuid(), Name = "World1", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World2", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World3", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World4", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World5", UserId = userId },
                new Environment2D { Id = Guid.NewGuid(), Name = "World6", UserId = userId }
            };

            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(existingWorlds);
            var newEnvironment = new Environment2D { Name = "World7" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(response.Result);
            var result = response.Result as BadRequestObjectResult;
            var problem = result.Value as ProblemDetails;
            Assert.AreEqual("Maximum aantal 2D-werelden bereikt", problem.Detail);
        }

        #endregion

        #region AC3 - Naam moet uniek zijn per gebruiker

        [TestMethod]
        public async Task AddAsync_UniqueWorldName_CanAddNewWorld()
        {
            // Arrange
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(new List<Environment2D>());
            environment2DRepository.Setup(x => x.SelectByUserIdAndNameAsync(userId, "UniqueWorld")).ReturnsAsync(null as Environment2D);
            var newEnvironment = new Environment2D { Name = "UniqueWorld" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<CreatedAtRouteResult>(response.Result);
        }

        [TestMethod]
        public async Task AddAsync_DuplicateWorldName_CannotAddNewWorld()
        {
            // Arrange
            var userId = "user123";
            authenticationService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            environment2DRepository.Setup(x => x.SelectByUserIdAsync(userId)).ReturnsAsync(new List<Environment2D>());

            var existingWorld = new Environment2D { Id = Guid.NewGuid(), Name = "ExistingWorld", UserId = userId };
            environment2DRepository.Setup(x => x.SelectByUserIdAndNameAsync(userId, "ExistingWorld")).ReturnsAsync(existingWorld);

            var newEnvironment = new Environment2D { Name = "ExistingWorld" };

            // Act
            var response = await controller.AddAsync(newEnvironment);

            // Assert
            Assert.IsInstanceOfType<ConflictObjectResult>(response.Result);
            var result = response.Result as ConflictObjectResult;
            var problem = result.Value as ProblemDetails;
            Assert.AreEqual("Naam bestaat al", problem.Detail);
        }

        #endregion
    }
}