using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class AuthorizeCallbackEndpointTests
    {
        private readonly Mock<IEventService> _events;
        private readonly Mock<ILogger<AuthorizeCallbackEndpoint>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly Mock<IAuthorizeRequestValidator> _validator;
        private readonly Mock<IAuthorizeInteractionResponseGenerator> _interactionGenerator;
        private readonly Mock<IAuthorizeResponseGenerator> _authorizeResponseGenerator;
        private readonly Mock<IUserSession> _userSession;
        private readonly Mock<IConsentMessageStore> _consentMessageStore;
        private readonly Mock<IAuthorizationParametersMessageStore> _authorizationParametersMessageStore;
        private readonly AuthorizeCallbackEndpoint _subject;

        public AuthorizeCallbackEndpointTests()
        {
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<AuthorizeCallbackEndpoint>>();
            _options = new IdentityServerOptions();
            _validator = new Mock<IAuthorizeRequestValidator>();
            _interactionGenerator = new Mock<IAuthorizeInteractionResponseGenerator>();
            _authorizeResponseGenerator = new Mock<IAuthorizeResponseGenerator>();
            _userSession = new Mock<IUserSession>();
            _consentMessageStore = new Mock<IConsentMessageStore>();
            _authorizationParametersMessageStore = new Mock<IAuthorizationParametersMessageStore>();

            _subject = new AuthorizeCallbackEndpoint(
                _events.Object,
                _logger.Object,
                _options,
                _validator.Object,
                _interactionGenerator.Object,
                _authorizeResponseGenerator.Object,
                _userSession.Object,
                _consentMessageStore.Object,
                _authorizationParametersMessageStore.Object);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_http_method_should_return_405()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task ProcessAsync_with_null_consent_data_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));
            _userSession.Setup(x => x.GetUserAsync()).ReturnsAsync(user);
            
            _consentMessageStore.Setup(x => x.ReadAsync(It.IsAny<string>()))
                .ReturnsAsync(new Message<ConsentResponse>(new ConsentResponse()));

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<AuthorizeResult>();
            _consentMessageStore.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_with_valid_params_should_process_authorize_request()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));
            _userSession.Setup(x => x.GetUserAsync()).ReturnsAsync(user);

            var consentResponse = new ConsentResponse { 
                RememberConsent = true,
                ScopesValuesConsented = new string[] { "openid", "profile" }
            };
            _consentMessageStore.Setup(x => x.ReadAsync(It.IsAny<string>()))
                .ReturnsAsync(new Message<ConsentResponse>(consentResponse));

            var validatedRequest = new ValidatedAuthorizeRequest();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(validatedRequest);

            var authorizeResult = new AuthorizeResult();
            _authorizeResponseGenerator.Setup(x => x.CreateResponseAsync(It.IsAny<ValidatedAuthorizeRequest>()))
                .ReturnsAsync(authorizeResult);

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<AuthorizeResult>();
            _consentMessageStore.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
            _validator.Verify(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()), Times.Once);
            _authorizeResponseGenerator.Verify(x => x.CreateResponseAsync(It.IsAny<ValidatedAuthorizeRequest>()), Times.Once);
        }
    }
}
