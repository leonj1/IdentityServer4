using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class UserInfoRequestValidationResultTests
    {
        [Fact]
        public void Should_Create_With_Default_Values()
        {
            var result = new UserInfoRequestValidationResult();
            
            result.IsError.Should().BeFalse();
            result.Error.Should().BeNull();
            result.ErrorDescription.Should().BeNull();
            result.TokenValidationResult.Should().BeNull();
            result.Subject.Should().BeNull();
        }

        [Fact]
        public void Can_Set_TokenValidationResult()
        {
            var tokenResult = new TokenValidationResult();
            var result = new UserInfoRequestValidationResult 
            { 
                TokenValidationResult = tokenResult 
            };

            result.TokenValidationResult.Should().BeSameAs(tokenResult);
        }

        [Fact]
        public void Can_Set_Subject()
        {
            var subject = new ClaimsPrincipal();
            var result = new UserInfoRequestValidationResult 
            { 
                Subject = subject 
            };

            result.Subject.Should().BeSameAs(subject);
        }

        [Fact]
        public void Inherits_From_ValidationResult()
        {
            var result = new UserInfoRequestValidationResult
            {
                IsError = true,
                Error = "error",
                ErrorDescription = "description"
            };

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("error");
            result.ErrorDescription.Should().Be("description");
        }
    }
}
