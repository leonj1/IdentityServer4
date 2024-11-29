using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class IntrospectionRequestValidationResultTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateInstance_WithDefaultValues()
        {
            // Act
            var result = new IntrospectionRequestValidationResult();

            // Assert
            result.IsActive.Should().BeFalse();
            result.Parameters.Should().BeNull();
            result.Api.Should().BeNull();
            result.Claims.Should().BeNull();
            result.Token.Should().BeNull();
        }

        [Fact]
        public void WhenPropertiesSet_ShouldReturnExpectedValues()
        {
            // Arrange
            var parameters = new NameValueCollection { { "key", "value" } };
            var api = new ApiResource("test_api");
            var claims = new List<Claim> { new Claim("type", "value") };
            const string token = "test_token";

            // Act
            var result = new IntrospectionRequestValidationResult
            {
                Parameters = parameters,
                Api = api,
                IsActive = true,
                Claims = claims,
                Token = token
            };

            // Assert
            result.Parameters.Should().NotBeNull();
            result.Parameters["key"].Should().Be("value");
            result.Api.Should().Be(api);
            result.IsActive.Should().BeTrue();
            result.Claims.Should().BeEquivalentTo(claims);
            result.Token.Should().Be(token);
        }

        [Fact]
        public void InheritanceTest_ShouldInheritFrom_ValidationResult()
        {
            // Arrange & Act
            var result = new IntrospectionRequestValidationResult();

            // Assert
            result.Should().BeAssignableTo<ValidationResult>();
        }

        [Theory]
        [InlineData(IntrospectionRequestValidationFailureReason.None, "None")]
        [InlineData(IntrospectionRequestValidationFailureReason.MissingToken, "MissingToken")]
        [InlineData(IntrospectionRequestValidationFailureReason.InvalidToken, "InvalidToken")]
        [InlineData(IntrospectionRequestValidationFailureReason.InvalidScope, "InvalidScope")]
        public void FailureReasonEnum_ShouldHaveExpectedValues(
            IntrospectionRequestValidationFailureReason reason, 
            string expectedString)
        {
            // Assert
            reason.ToString().Should().Be(expectedString);
        }
    }
}
