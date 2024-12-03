using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class AuthorizeEndpointTests
    {
        private readonly Mock<IEventService> _events;
        private readonly Mock<ILogger<AuthorizeEndpoint>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly Mock<IAuthorizeRequestValidator> _validator;
        private readonly Mock<IAuthorizeInteractionResponseGenerator> _interactionGenerator;
        private readonly Mock<IAuthorizeResponseGenerator> _authorizeResponseGenerator;
        private readonly Mock<IUserSession> _userSession;
        private readonly AuthorizeEndpoint _subject;

        public AuthorizeEndpointTests()
        {
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<AuthorizeEndpoint>>();
            _options = new IdentityServerOptions();
            _validator = new Mock<IAuthorizeRequestValidator>();
            _interactionGenerator = new Mock<IAuthorizeInteractionResponseGenerator>();
            _authorizeResponseGenerator = new Mock<IAuthorizeResponseGenerator>();
            _userSession = new Mock<IUserSession>();

            _subject = new AuthorizeEndpoint(
                _events.Object,
                _logger.Object,
                _options,
                _validator.Object,
                _interactionGenerator.Object,
                _authorizeResponseGenerator.Object,
                _userSession.Object);
        }

        [Fact]
        public async Task Get_request_should_return_valid_result()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.QueryString = new QueryString("?client_id=test&scope=openid");

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Post_request_without_form_content_type_should_return_unsupported_media_type()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Invalid_method_should_return_method_not_allowed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "PUT";

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task Post_request_with_form_content_should_return_valid_result()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";
            
            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
