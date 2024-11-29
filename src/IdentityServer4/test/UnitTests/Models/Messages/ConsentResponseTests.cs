using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models.Messages
{
    public class ConsentResponseTests
    {
        [Fact]
        public void Granted_Should_ReturnFalse_When_ScopesValuesConsented_IsNull()
        {
            // Arrange
            var response = new ConsentResponse
            {
                ScopesValuesConsented = null
            };

            // Act & Assert
            response.Granted.Should().BeFalse();
        }

        [Fact]
        public void Granted_Should_ReturnFalse_When_ScopesValuesConsented_IsEmpty()
        {
            // Arrange
            var response = new ConsentResponse
            {
                ScopesValuesConsented = Enumerable.Empty<string>()
            };

            // Act & Assert
            response.Granted.Should().BeFalse();
        }

        [Fact]
        public void Granted_Should_ReturnFalse_When_Error_IsNotNull()
        {
            // Arrange
            var response = new ConsentResponse
            {
                ScopesValuesConsented = new[] { "scope1" },
                Error = AuthorizationError.AccessDenied
            };

            // Act & Assert
            response.Granted.Should().BeFalse();
        }

        [Fact]
        public void Granted_Should_ReturnTrue_When_ValidScopesAndNoError()
        {
            // Arrange
            var response = new ConsentResponse
            {
                ScopesValuesConsented = new[] { "scope1" },
                Error = null
            };

            // Act & Assert
            response.Granted.Should().BeTrue();
        }

        [Fact]
        public void Properties_Should_BeSettable()
        {
            // Arrange
            var response = new ConsentResponse
            {
                Error = AuthorizationError.ConsentRequired,
                ErrorDescription = "Consent is required",
                RememberConsent = true,
                Description = "Test device",
                ScopesValuesConsented = new[] { "scope1", "scope2" }
            };

            // Assert
            response.Error.Should().Be(AuthorizationError.ConsentRequired);
            response.ErrorDescription.Should().Be("Consent is required");
            response.RememberConsent.Should().BeTrue();
            response.Description.Should().Be("Test device");
            response.ScopesValuesConsented.Should().BeEquivalentTo(new[] { "scope1", "scope2" });
        }
    }
}
