using IdentityModel;
using IdentityServer4.ResponseHandling;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.ResponseHandling
{
    public class TokenErrorResponseTests
    {
        [Fact]
        public void Default_Constructor_Should_Set_Default_Values()
        {
            // Act
            var response = new TokenErrorResponse();

            // Assert
            response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
            response.ErrorDescription.Should().BeNull();
            response.Custom.Should().NotBeNull();
            response.Custom.Should().BeEmpty();
        }

        [Fact]
        public void Setting_Properties_Should_Work()
        {
            // Arrange
            var response = new TokenErrorResponse
            {
                Error = "custom_error",
                ErrorDescription = "custom description",
            };
            response.Custom.Add("custom_key", "custom_value");

            // Assert
            response.Error.Should().Be("custom_error");
            response.ErrorDescription.Should().Be("custom description");
            response.Custom.Should().ContainKey("custom_key");
            response.Custom["custom_key"].Should().Be("custom_value");
        }
    }
}
