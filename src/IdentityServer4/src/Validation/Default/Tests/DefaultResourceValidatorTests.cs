using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class DefaultResourceValidatorTests
    {
        private readonly Mock<IResourceStore> _mockStore;
        private readonly Mock<IScopeParser> _mockScopeParser;
        private readonly Mock<ILogger<DefaultResourceValidator>> _mockLogger;
        private readonly DefaultResourceValidator _validator;

        public DefaultResourceValidatorTests()
        {
            _mockStore = new Mock<IResourceStore>();
            _mockScopeParser = new Mock<IScopeParser>();
            _mockLogger = new Mock<ILogger<DefaultResourceValidator>>();
            _validator = new DefaultResourceValidator(_mockStore.Object, _mockScopeParser.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestedResourcesAsync(null));
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_WhenScopeParsingFails_ReturnsInvalidResult()
        {
            // Arrange
            var request = new ResourceValidationRequest
            {
                Client = new Client { ClientId = "test_client" },
                Scopes = new[] { "invalid_scope" }
            };

            _mockScopeParser.Setup(x => x.ParseScopeValues(It.IsAny<IEnumerable<string>>()))
                .Returns(new ParsedScopeValidationResult { Succeeded = false, Errors = new[] { new ParsedScopeValidationError { RawValue = "invalid_scope", Error = "Invalid scope" } } });

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            Assert.Contains("invalid_scope", result.InvalidScopes);
            Assert.Empty(result.Resources.IdentityResources);
            Assert.Empty(result.Resources.ApiResources);
        }

        [Fact]
        public async Task ValidateRequestedResourcesAsync_WhenOfflineAccessRequested_AndClientAllowed_AddsToResult()
        {
            // Arrange
            var identityResource = new IdentityResource { Name = "openid" };
            var client = new Client 
            { 
                ClientId = "test_client",
                AllowedScopes = new[] { "openid" }
            };

            var request = new ResourceValidationRequest
            {
                Client = client,
                Scopes = new[] { "openid" }
            };

            _mockScopeParser.Setup(x => x.ParseScopeValues(It.IsAny<IEnumerable<string>>()))
                .Returns(new ParsedScopeValidationResult 
                { 
                    Succeeded = true,
                    ParsedScopes = new[] { new ParsedScopeValue("openid") }
                });

            _mockStore.Setup(x => x.FindEnabledResourcesByScopeAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new Resources(new[] { identityResource }, Enumerable.Empty<ApiResource>(), Enumerable.Empty<ApiScope>()));

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            Assert.Contains(identityResource, result.Resources.IdentityResources);
            Assert.Empty(result.InvalidScopes);
        }
    }
}
