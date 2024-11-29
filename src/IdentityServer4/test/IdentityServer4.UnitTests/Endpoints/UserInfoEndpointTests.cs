using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Validation;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Endpoints
{
    public class UserInfoEndpointTests
    {
        private readonly Mock<BearerTokenUsageValidator> _tokenValidator;
        private readonly Mock<IUserInfoRequestValidator> _requestValidator;
        private readonly Mock<IUserInfoResponseGenerator> _responseGenerator;
        private readonly Mock<ILogger<UserInfoEndpoint>> _logger;
        private readonly UserInfoEndpoint _target;
        private readonly HttpContext _context;

        public UserInfoEndpointTests()
        {
            _tokenValidator = new Mock<BearerTokenUsageValidator>();
            _requestValidator = new Mock<IUserInfoRequestValidator>();
            _responseGenerator = new Mock<IUserInfoResponseGenerator>();
            _logger = new Mock<ILogger<UserInfoEndpoint>>();
            
            _target = new UserInfoEndpoint(
                _tokenValidator.Object,
                _requestValidator.Object,
                _responseGenerator.Object,
                _logger.Object);

            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_method_should_return_405()
        {
            // Arrange
            _context.Request.Method = "PUT";

            // Act
            var result = await _target.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = (StatusCodeResult)result;
            statusResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task ProcessAsync_without_token_should_return_error()
        {
            // Arrange
            _context.Request.Method = "GET";
            _tokenValidator.Setup(v => v.ValidateAsync(_context))
                .ReturnsAsync(new BearerTokenUsageValidationResult { TokenFound = false });

            // Act
            var result = await _target.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<ProtectedResourceErrorResult>();
            var errorResult = (ProtectedResourceErrorResult)result;
            errorResult.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        public async Task ProcessAsync_with_validation_error_should_return_error()
        {
            // Arrange
            _context.Request.Method = "GET";
            _tokenValidator.Setup(v => v.ValidateAsync(_context))
                .ReturnsAsync(new BearerTokenUsageValidationResult { TokenFound = true, Token = "token" });
            
            _requestValidator.Setup(v => v.ValidateRequestAsync("token"))
                .ReturnsAsync(new UserInfoRequestValidationResult { IsError = true, Error = "validation_error" });

            // Act
            var result = await _target.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<ProtectedResourceErrorResult>();
            var errorResult = (ProtectedResourceErrorResult)result;
            errorResult.Error.Should().Be("validation_error");
        }

        [Fact]
        public async Task ProcessAsync_with_valid_request_should_return_userinfo()
        {
            // Arrange
            _context.Request.Method = "GET";
            var validationResult = new UserInfoRequestValidationResult { IsError = false };
            var responseData = new Dictionary<string, object> { { "sub", "123" } };

            _tokenValidator.Setup(v => v.ValidateAsync(_context))
                .ReturnsAsync(new BearerTokenUsageValidationResult { TokenFound = true, Token = "token" });
            
            _requestValidator.Setup(v => v.ValidateRequestAsync("token"))
                .ReturnsAsync(validationResult);
            
            _responseGenerator.Setup(g => g.ProcessAsync(validationResult))
                .ReturnsAsync(responseData);

            // Act
            var result = await _target.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<UserInfoResult>();
            var userInfoResult = (UserInfoResult)result;
            userInfoResult.Claims.Should().NotBeNull();
            userInfoResult.Claims.Should().ContainKey("sub");
            userInfoResult.Claims["sub"].Should().Be("123");
        }
    }
}
