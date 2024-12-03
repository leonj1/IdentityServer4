using FluentAssertions;
using IdentityServer4.ResponseHandling;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class TokenRevocationResponseTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            var response = new TokenRevocationResponse();
            
            response.Success.Should().BeFalse();
            response.TokenType.Should().BeNull();
            response.Error.Should().BeNull();
        }

        [Fact]
        public void SettingProperties_ShouldWork()
        {
            var response = new TokenRevocationResponse
            {
                Success = true,
                TokenType = "access_token",
                Error = "invalid_token"
            };

            response.Success.Should().BeTrue();
            response.TokenType.Should().Be("access_token");
            response.Error.Should().Be("invalid_token");
        }
    }
}
