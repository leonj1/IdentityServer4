using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class ApiSecretValidatorTests
    {
        private readonly Mock<IResourceStore> _resourceStore;
        private readonly Mock<ISecretsListParser> _parser;
        private readonly Mock<ISecretsListValidator> _validator;
        private readonly Mock<IEventService> _events;
        private readonly Mock<ILogger<ApiSecretValidator>> _logger;
        private readonly ApiSecretValidator _subject;

        public ApiSecretValidatorTests()
        {
            _resourceStore = new Mock<IResourceStore>();
            _parser = new Mock<ISecretsListParser>();
            _validator = new Mock<ISecretsListValidator>();
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<ApiSecretValidator>>();

            _subject = new ApiSecretValidator(
                _resourceStore.Object,
                _parser.Object,
                _validator.Object,
                _events.Object,
                _logger.Object);
        }

        [Fact]
        public async Task No_Secret_Should_Return_Failure()
        {
            // Arrange
            _parser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync((ParsedSecret)null);

            // Act
            var result = await _subject.ValidateAsync(new DefaultHttpContext());

            // Assert
            result.IsError.Should().BeTrue();
            _events.Verify(x => x.RaiseAsync(It.IsAny<ApiAuthenticationFailureEvent>()), Times.Once);
        }

        [Fact]
        public async Task Invalid_Api_Should_Return_Failure()
        {
            // Arrange
            var parsedSecret = new ParsedSecret { Id = "unknown_api" };
            _parser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            
            _resourceStore.Setup(x => x.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<ApiResource>());

            // Act
            var result = await _subject.ValidateAsync(new DefaultHttpContext());

            // Assert
            result.IsError.Should().BeTrue();
            _events.Verify(x => x.RaiseAsync(It.IsAny<ApiAuthenticationFailureEvent>()), Times.Once);
        }

        [Fact]
        public async Task Disabled_Api_Should_Return_Failure()
        {
            // Arrange
            var parsedSecret = new ParsedSecret { Id = "disabled_api" };
            _parser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            
            var api = new ApiResource
            {
                Name = "disabled_api",
                Enabled = false
            };
            
            _resourceStore.Setup(x => x.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new[] { api });

            // Act
            var result = await _subject.ValidateAsync(new DefaultHttpContext());

            // Assert
            result.IsError.Should().BeTrue();
            _events.Verify(x => x.RaiseAsync(It.IsAny<ApiAuthenticationFailureEvent>()), Times.Once);
        }

        [Fact]
        public async Task Valid_Api_With_Valid_Secret_Should_Return_Success()
        {
            // Arrange
            var parsedSecret = new ParsedSecret 
            { 
                Id = "api1",
                Type = "secret"
            };
            
            _parser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            
            var api = new ApiResource
            {
                Name = "api1",
                Enabled = true,
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) }
            };
            
            _resourceStore.Setup(x => x.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new[] { api });
                
            _validator.Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = true });

            // Act
            var result = await _subject.ValidateAsync(new DefaultHttpContext());

            // Assert
            result.IsError.Should().BeFalse();
            result.Resource.Should().Be(api);
            _events.Verify(x => x.RaiseAsync(It.IsAny<ApiAuthenticationSuccessEvent>()), Times.Once);
        }

        [Fact]
        public async Task Multiple_Apis_Should_Return_Failure()
        {
            // Arrange
            var parsedSecret = new ParsedSecret { Id = "api1" };
            _parser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(parsedSecret);
            
            var apis = new[]
            {
                new ApiResource { Name = "api1" },
                new ApiResource { Name = "api1" }
            };
            
            _resourceStore.Setup(x => x.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(apis);

            // Act
            var result = await _subject.ValidateAsync(new DefaultHttpContext());

            // Assert
            result.IsError.Should().BeTrue();
            _events.Verify(x => x.RaiseAsync(It.IsAny<ApiAuthenticationFailureEvent>()), Times.Once);
        }
    }
}
