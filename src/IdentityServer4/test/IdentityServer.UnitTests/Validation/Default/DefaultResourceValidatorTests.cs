using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Validation.Default
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
        public async Task ValidateRequestedResources_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestedResourcesAsync(null));
        }

        [Fact]
        public async Task ValidateRequestedResources_WhenScopeParsingFails_ShouldReturnInvalidResult()
        {
            // Arrange
            var request = new ResourceValidationRequest { Scopes = new[] { "scope1" } };
            var parsedResult = new ParsedScopesResult(new[] { 
                new ParsedScopeValidationError { RawValue = "scope1", Error = "Invalid scope" } 
            });
            
            _mockScopeParser.Setup(x => x.ParseScopeValues(It.IsAny<IEnumerable<string>>()))
                .Returns(parsedResult);

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.InvalidScopes.Should().Contain("scope1");
            result.Resources.IdentityResources.Should().BeEmpty();
            result.Resources.ApiResources.Should().BeEmpty();
            result.Resources.ApiScopes.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateRequestedResources_WhenOfflineAccessRequested_AndClientAllowed_ShouldAddToResult()
        {
            // Arrange
            var client = new Client { AllowOfflineAccess = true };
            var request = new ResourceValidationRequest 
            { 
                Client = client,
                Scopes = new[] { IdentityServerConstants.StandardScopes.OfflineAccess } 
            };

            _mockScopeParser.Setup(x => x.ParseScopeValues(It.IsAny<IEnumerable<string>>()))
                .Returns(new ParsedScopesResult(new[] { 
                    new ParsedScopeValue(IdentityServerConstants.StandardScopes.OfflineAccess)
                }));

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.Resources.OfflineAccess.Should().BeTrue();
            result.InvalidScopes.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateRequestedResources_WhenIdentityResourceRequested_AndClientAllowed_ShouldAddToResult()
        {
            // Arrange
            var identityResource = new IdentityResource { Name = "openid" };
            var client = new Client { AllowedScopes = new[] { "openid" } };
            var request = new ResourceValidationRequest 
            { 
                Client = client,
                Scopes = new[] { "openid" } 
            };

            _mockScopeParser.Setup(x => x.ParseScopeValues(It.IsAny<IEnumerable<string>>()))
                .Returns(new ParsedScopesResult(new[] { new ParsedScopeValue("openid") }));

            _mockStore.Setup(x => x.FindEnabledResourcesByScopeAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new Resources(new[] { identityResource }, 
                    Enumerable.Empty<ApiResource>(), 
                    Enumerable.Empty<ApiScope>()));

            // Act
            var result = await _validator.ValidateRequestedResourcesAsync(request);

            // Assert
            result.Resources.IdentityResources.Should().Contain(identityResource);
            result.InvalidScopes.Should().BeEmpty();
        }
    }
}
