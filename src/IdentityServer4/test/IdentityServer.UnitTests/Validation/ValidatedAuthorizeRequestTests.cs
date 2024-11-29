using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ValidatedAuthorizeRequestTests
    {
        [Fact]
        public void AccessTokenRequested_ShouldReturnTrue_ForValidResponseTypes()
        {
            // Arrange
            var validResponseTypes = new[]
            {
                OidcConstants.ResponseTypes.IdTokenToken,
                OidcConstants.ResponseTypes.Code,
                OidcConstants.ResponseTypes.CodeIdToken,
                OidcConstants.ResponseTypes.CodeToken,
                OidcConstants.ResponseTypes.CodeIdTokenToken
            };

            foreach (var responseType in validResponseTypes)
            {
                var request = new ValidatedAuthorizeRequest
                {
                    ResponseType = responseType
                };

                // Act
                var result = request.AccessTokenRequested;

                // Assert
                result.Should().BeTrue($"Response type '{responseType}' should request access token");
            }
        }

        [Fact]
        public void AccessTokenRequested_ShouldReturnFalse_ForInvalidResponseTypes()
        {
            // Arrange
            var invalidResponseTypes = new[]
            {
                OidcConstants.ResponseTypes.IdToken,
                "invalid_type",
                string.Empty,
                null
            };

            foreach (var responseType in invalidResponseTypes)
            {
                var request = new ValidatedAuthorizeRequest
                {
                    ResponseType = responseType
                };

                // Act
                var result = request.AccessTokenRequested;

                // Assert
                result.Should().BeFalse($"Response type '{responseType}' should not request access token");
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeCollections()
        {
            // Arrange & Act
            var request = new ValidatedAuthorizeRequest();

            // Assert
            request.RequestedScopes.Should().NotBeNull();
            request.RequestedScopes.Should().BeEmpty();
            request.AuthenticationContextReferenceClasses.Should().NotBeNull();
            request.AuthenticationContextReferenceClasses.Should().BeEmpty();
            request.PromptModes.Should().NotBeNull();
            request.PromptModes.Should().BeEmpty();
            request.RequestObjectValues.Should().NotBeNull();
            request.RequestObjectValues.Should().BeEmpty();
        }
    }
}
