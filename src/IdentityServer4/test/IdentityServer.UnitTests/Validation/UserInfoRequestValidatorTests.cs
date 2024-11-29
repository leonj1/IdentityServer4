using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class UserInfoRequestValidatorTests
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IProfileService _profileService;
        private readonly UserInfoRequestValidator _validator;
        private readonly ILogger<UserInfoRequestValidator> _logger;

        public UserInfoRequestValidatorTests()
        {
            _tokenValidator = new MockTokenValidator();
            _profileService = new MockProfileService();
            _logger = TestLogger.Create<UserInfoRequestValidator>();
            _validator = new UserInfoRequestValidator(_tokenValidator, _profileService, _logger);
        }

        [Fact]
        public async Task Valid_Token_Should_Return_Success()
        {
            // Arrange
            var token = "valid_token";

            // Act
            var result = await _validator.ValidateRequestAsync(token);

            // Assert
            result.IsError.Should().BeFalse();
            result.Subject.Should().NotBeNull();
        }

        [Fact]
        public async Task Invalid_Token_Should_Return_Error()
        {
            // Arrange
            var token = "invalid_token";

            // Act
            var result = await _validator.ValidateRequestAsync(token);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Token_Without_SubClaim_Should_Return_Error()
        {
            // Arrange
            var token = "token_without_sub";

            // Act
            var result = await _validator.ValidateRequestAsync(token);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        public async Task Inactive_User_Should_Return_Error()
        {
            // Arrange
            var token = "inactive_user_token";

            // Act
            var result = await _validator.ValidateRequestAsync(token);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }
    }

    internal class MockTokenValidator : ITokenValidator
    {
        public Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null)
        {
            var result = new TokenValidationResult();

            switch (token)
            {
                case "valid_token":
                    result.IsError = false;
                    result.Claims = new List<Claim> 
                    { 
                        new Claim(JwtClaimTypes.Subject, "123"),
                        new Claim(JwtClaimTypes.Scope, "openid")
                    };
                    result.Client = new Client();
                    break;

                case "token_without_sub":
                    result.IsError = false;
                    result.Claims = new List<Claim> 
                    { 
                        new Claim(JwtClaimTypes.Scope, "openid")
                    };
                    result.Client = new Client();
                    break;

                case "inactive_user_token":
                    result.IsError = false;
                    result.Claims = new List<Claim> 
                    { 
                        new Claim(JwtClaimTypes.Subject, "inactive"),
                        new Claim(JwtClaimTypes.Scope, "openid")
                    };
                    result.Client = new Client();
                    break;

                default:
                    result.IsError = true;
                    result.Error = "invalid_token";
                    break;
            }

            return Task.FromResult(result);
        }

        public Task<TokenValidationResult> ValidateIdentityTokenAsync(string token, string clientId = null, bool validateLifetime = true)
        {
            throw new NotImplementedException();
        }

        public Task<TokenValidationResult> ValidateRefreshTokenAsync(string token, Client client = null)
        {
            throw new NotImplementedException();
        }
    }

    internal class MockProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = context.Subject.GetSubjectId() != "inactive";
            return Task.CompletedTask;
        }
    }
}
