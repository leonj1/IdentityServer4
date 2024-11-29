using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ClientSecretValidatorTests
    {
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<ISecretsListParser> _mockParser;
        private readonly Mock<ISecretsListValidator> _mockValidator;
        private readonly Mock<IEventService> _mockEvents;
        private readonly Mock<ILogger<ClientSecretValidator>> _mockLogger;
        private readonly ClientSecretValidator _validator;

        public ClientSecretValidatorTests()
        {
            _mockClientStore = new Mock<IClientStore>();
            _mockParser = new Mock<ISecretsListParser>();
            _mockValidator = new Mock<ISecretsListValidator>();
            _mockEvents = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<ClientSecretValidator>>();

            _validator = new ClientSecretValidator(
                _mockClientStore.Object,
                _mockParser.Object,
                _mockValidator.Object,
                _mockEvents.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ValidateAsync_WhenNoClientIdFound_ShouldReturnError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync((ParsedSecret)null);

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.IsError.Should().BeTrue();
            result.Client.Should().BeNull();
        }

        [Fact]
        public async Task ValidateAsync_WhenClientNotFound_ShouldReturnError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var parsedSecret = new ParsedSecret { Id = "client1" };
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            _mockClientStore.Setup(x => x.FindEnabledClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Client)null);

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.IsError.Should().BeTrue();
            result.Client.Should().BeNull();
        }

        [Fact]
        public async Task ValidateAsync_WhenPublicClient_ShouldSkipSecretValidation()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var parsedSecret = new ParsedSecret { Id = "client1" };
            var client = new Client { ClientId = "client1", RequireClientSecret = false };
            
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            _mockClientStore.Setup(x => x.FindEnabledClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(client);

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.IsError.Should().BeFalse();
            result.Client.Should().Be(client);
            _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()), Times.Never);
        }

        [Fact]
        public async Task ValidateAsync_WhenValidSecretProvided_ShouldSucceed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var parsedSecret = new ParsedSecret { Id = "client1" };
            var client = new Client 
            { 
                ClientId = "client1", 
                RequireClientSecret = true,
                ClientSecrets = { new Secret("secret".Sha256()) }
            };
            
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            _mockClientStore.Setup(x => x.FindEnabledClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(client);
            _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = true });

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.IsError.Should().BeFalse();
            result.Client.Should().Be(client);
        }

        [Fact]
        public async Task ValidateAsync_WhenInvalidSecretProvided_ShouldReturnError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var parsedSecret = new ParsedSecret { Id = "client1" };
            var client = new Client 
            { 
                ClientId = "client1", 
                RequireClientSecret = true,
                ClientSecrets = { new Secret("secret".Sha256()) }
            };
            
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            _mockClientStore.Setup(x => x.FindEnabledClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(client);
            _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = false });

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.IsError.Should().BeTrue();
            result.Client.Should().BeNull();
        }
    }
}
