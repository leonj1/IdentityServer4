using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class TokenRequestValidationResultTests
    {
        [Fact]
        public void Constructor_WithValidRequest_SetsPropertiesCorrectly()
        {
            // Arrange
            var validatedRequest = new ValidatedTokenRequest();
            var customResponse = new Dictionary<string, object> { { "key", "value" } };

            // Act
            var result = new TokenRequestValidationResult(validatedRequest, customResponse);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.Should().BeSameAs(validatedRequest);
            result.CustomResponse.Should().BeSameAs(customResponse);
            result.Error.Should().BeNull();
            result.ErrorDescription.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithError_SetsPropertiesCorrectly()
        {
            // Arrange
            var validatedRequest = new ValidatedTokenRequest();
            var customResponse = new Dictionary<string, object> { { "key", "value" } };
            var error = "test_error";
            var errorDescription = "test error description";

            // Act
            var result = new TokenRequestValidationResult(validatedRequest, error, errorDescription, customResponse);

            // Assert
            result.IsError.Should().BeTrue();
            result.ValidatedRequest.Should().BeSameAs(validatedRequest);
            result.CustomResponse.Should().BeSameAs(customResponse);
            result.Error.Should().Be(error);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        [Fact]
        public void CustomResponse_CanBeModifiedAfterConstruction()
        {
            // Arrange
            var result = new TokenRequestValidationResult(new ValidatedTokenRequest());
            var newCustomResponse = new Dictionary<string, object> { { "new_key", "new_value" } };

            // Act
            result.CustomResponse = newCustomResponse;

            // Assert
            result.CustomResponse.Should().BeSameAs(newCustomResponse);
        }
    }
}
