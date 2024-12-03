using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class TokenRevocationRequestValidationResultTests
    {
        [Fact]
        public void Should_Create_With_Default_Values()
        {
            var result = new TokenRevocationRequestValidationResult();
            
            result.IsError.Should().BeFalse();
            result.Error.Should().BeNull();
            result.TokenTypeHint.Should().BeNull();
            result.Token.Should().BeNull();
            result.Client.Should().BeNull();
        }

        [Fact]
        public void Should_Set_And_Get_Properties()
        {
            var client = new Client();
            var result = new TokenRevocationRequestValidationResult
            {
                IsError = true,
                Error = "error",
                TokenTypeHint = "hint",
                Token = "token",
                Client = client
            };

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("error");
            result.TokenTypeHint.Should().Be("hint");
            result.Token.Should().Be("token");
            result.Client.Should().BeSameAs(client);
        }

        [Fact]
        public void Should_Inherit_From_ValidationResult()
        {
            var result = new TokenRevocationRequestValidationResult();
            result.Should().BeAssignableTo<ValidationResult>();
        }
    }
}
