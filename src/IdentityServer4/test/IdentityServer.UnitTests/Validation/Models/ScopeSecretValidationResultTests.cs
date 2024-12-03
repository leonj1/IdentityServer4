using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class ApiSecretValidationResultTests
    {
        [Fact]
        public void Should_Create_Success_Result()
        {
            // Arrange
            var resource = new ApiResource("api1");
            
            // Act
            var result = new ApiSecretValidationResult
            {
                Resource = resource,
                Success = true
            };

            // Assert
            result.Success.Should().BeTrue();
            result.Resource.Should().NotBeNull();
            result.Resource.Name.Should().Be("api1");
        }

        [Fact]
        public void Should_Create_Failure_Result()
        {
            // Act
            var result = new ApiSecretValidationResult
            {
                Success = false,
                Error = "Invalid secret"
            };

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Should().Be("Invalid secret");
            result.Resource.Should().BeNull();
        }
    }
}
