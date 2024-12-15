using System;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using IdentityModel;

namespace IdentityServer4.UnitTests.Endpoints.Results
{
    public class AuthorizeResultTests
    {
        private readonly Mock<IdentityServerOptions> _options;
        private readonly Mock<IUserSession> _userSession;
        private readonly Mock<IMessageStore<ErrorMessage>> _errorMessageStore;
        private readonly Mock<ISystemClock> _clock;
        private readonly HttpContext _context;

        public AuthorizeResultTests()
        {
            _options = new Mock<IdentityServerOptions>();
            _userSession = new Mock<IUserSession>();
            _errorMessageStore = new Mock<IMessageStore<ErrorMessage>>();
            _clock = new Mock<ISystemClock>();
            _context = new DefaultHttpContext();
        }

        [Fact]
        public void Constructor_WhenResponseIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AuthorizeResult(null));
        }

        [Fact]
        public async Task ExecuteAsync_WhenSuccessfulResponse_AddsClientIdToUserSession()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Request = new ValidatedAuthorizeRequest
                {
                    ClientId = "test_client",
                    ResponseMode = OidcConstants.ResponseModes.Query
                },
                RedirectUri = "https://client.com/callback"
            };

            var result = new AuthorizeResult(
                response,
                _options.Object,
                _userSession.Object,
                _errorMessageStore.Object,
                _clock.Object
            );

            // Act
            await result.ExecuteAsync(_context);

            // Assert
            _userSession.Verify(x => x.AddClientIdAsync("test_client"), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithFormPostMode_ReturnsHtmlForm()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Request = new ValidatedAuthorizeRequest
                {
                    ClientId = "test_client",
                    ResponseMode = OidcConstants.ResponseModes.FormPost,
                    RedirectUri = "https://client.com/callback"
                }
            };

            var result = new AuthorizeResult(
                response,
                _options.Object,
                _userSession.Object,
                _errorMessageStore.Object,
                _clock.Object
            );

            // Act
            await result.ExecuteAsync(_context);

            // Assert
            Assert.Equal("text/html; charset=UTF-8", _context.Response.ContentType);
        }

        [Fact]
        public async Task ExecuteAsync_WithSafeError_RedirectsToClient()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Error = OidcConstants.AuthorizeErrors.AccessDenied,
                Request = new ValidatedAuthorizeRequest
                {
                    ClientId = "test_client",
                    ResponseMode = OidcConstants.ResponseModes.Query
                },
                RedirectUri = "https://client.com/callback"
            };

            var result = new AuthorizeResult(
                response,
                _options.Object,
                _userSession.Object,
                _errorMessageStore.Object,
                _clock.Object
            );

            // Act
            await result.ExecuteAsync(_context);

            // Assert
            Assert.Equal(302, _context.Response.StatusCode);
            Assert.Contains("error=access_denied", _context.Response.Headers["Location"].ToString());
        }
    }
}
