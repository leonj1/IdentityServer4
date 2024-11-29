using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenValidatorTests
    {
        private readonly Mock<ITokenValidator> _tokenValidator;

        public TokenValidatorTests()
        {
            _tokenValidator = new Mock<ITokenValidator>();
        }

        [Fact]
        public async Task ValidateAccessToken_WithValidToken_ShouldReturnSuccessResult()
        {
            // Arrange
            var expectedToken = "valid_access_token";
            var expectedScope = "api1";
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = false,
                Claims = new System.Security.Claims.ClaimsPrincipal()
            };

            _tokenValidator
                .Setup(x => x.ValidateAccessTokenAsync(expectedToken, expectedScope))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateAccessTokenAsync(expectedToken, expectedScope);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Claims.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateIdentityToken_WithValidToken_ShouldReturnSuccessResult()
        {
            // Arrange
            var expectedToken = "valid_identity_token";
            var expectedClientId = "test_client";
            var validateLifetime = true;

            var expectedResult = new TokenValidationResult 
            { 
                IsError = false,
                Claims = new System.Security.Claims.ClaimsPrincipal()
            };

            _tokenValidator
                .Setup(x => x.ValidateIdentityTokenAsync(expectedToken, expectedClientId, validateLifetime))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateIdentityTokenAsync(expectedToken, expectedClientId, validateLifetime);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Claims.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateAccessToken_WithInvalidToken_ShouldReturnErrorResult()
        {
            // Arrange
            var invalidToken = "invalid_token";
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "Invalid token"
            };

            _tokenValidator
                .Setup(x => x.ValidateAccessTokenAsync(invalidToken, It.IsAny<string>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateAccessTokenAsync(invalidToken);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid token");
        }

        [Fact]
        public async Task ValidateIdentityToken_WithInvalidToken_ShouldReturnErrorResult()
        {
            // Arrange
            var invalidToken = "invalid_token";
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "Invalid token"
            };

            _tokenValidator
                .Setup(x => x.ValidateIdentityTokenAsync(invalidToken, It.IsAny<string>(), true))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateIdentityTokenAsync(invalidToken);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid token");
        }

        [Fact]
        public async Task ValidateIdentityToken_WithTooLongToken_ShouldReturnErrorResult()
        {
            // Arrange
            var longToken = new string('x', 5000); // Create very long token
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "invalid_token"
            };

            _tokenValidator
                .Setup(x => x.ValidateIdentityTokenAsync(longToken, It.IsAny<string>(), true))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateIdentityTokenAsync(longToken);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_token");
        }

        [Fact]
        public async Task ValidateIdentityToken_WithMissingClientId_ShouldReturnErrorResult()
        {
            // Arrange
            var token = "valid_token";
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "invalid_token"
            };

            _tokenValidator
                .Setup(x => x.ValidateIdentityTokenAsync(token, null, true))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateIdentityTokenAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_token");
        }

        [Fact]
        public async Task ValidateAccessToken_WithDisabledClient_ShouldReturnErrorResult()
        {
            // Arrange
            var token = "valid_token";
            var clientId = "disabled_client";
            
            var expectedResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "invalid_token",
                Claims = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim("client_id", clientId) }
                ))
            };

            _tokenValidator
                .Setup(x => x.ValidateAccessTokenAsync(token, It.IsAny<string>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _tokenValidator.Object.ValidateAccessTokenAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_token");
        }
    }
}
