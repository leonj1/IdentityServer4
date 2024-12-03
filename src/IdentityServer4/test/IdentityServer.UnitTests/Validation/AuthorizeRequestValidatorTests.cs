using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class AuthorizeRequestValidatorTests
    {
        private readonly IAuthorizeRequestValidator _validator;
        private readonly ClaimsPrincipal _subject;
        
        public AuthorizeRequestValidatorTests()
        {
            _validator = new TestAuthorizeRequestValidator();
            _subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));
        }

        [Fact]
        public async Task ValidateAsync_WithValidParameters_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "client_id", "test_client" },
                { "response_type", "code" },
                { "scope", "openid profile" }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters, _subject);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WithNullParameters_ShouldFail()
        {
            // Act
            var result = await _validator.ValidateAsync(null, _subject);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_WithoutSubject_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "client_id", "test_client" },
                { "response_type", "code" }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
        }
    }

    internal class TestAuthorizeRequestValidator : IAuthorizeRequestValidator
    {
        public Task<AuthorizeRequestValidationResult> ValidateAsync(NameValueCollection parameters, ClaimsPrincipal subject = null)
        {
            if (parameters == null)
            {
                return Task.FromResult(new AuthorizeRequestValidationResult { 
                    IsError = true,
                    Error = "Invalid parameters"
                });
            }

            return Task.FromResult(new AuthorizeRequestValidationResult());
        }
    }
}
