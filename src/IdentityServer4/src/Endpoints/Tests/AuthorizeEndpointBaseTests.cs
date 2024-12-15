using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Endpoints
{
    public class AuthorizeEndpointBaseTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ILogger<AuthorizeEndpointBase>> _mockLogger;
        private readonly Mock<IIdentityServerInteractionService> _mockInteractionService;
        private readonly Mock<IResourceValidator> _mockResourceValidator;
        private readonly Mock<IConsentService> _mockConsentService;
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly Mock<IAuthorizeResponseGenerator> _mockAuthorizeResponseGenerator;
        private readonly Mock<IEventService> _mockEventService2;

        public AuthorizeEndpointBaseTests()
        {
            _mockEventService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<AuthorizeEndpointBase>>();
            _mockInteractionService = new Mock<IIdentityServerInteractionService>();
            _mockResourceValidator = new Mock<IResourceValidator>();
            _mockConsentService = new Mock<IConsentService>();
            _mockProfileService = new Mock<IProfileService>();
            _mockAuthorizeResponseGenerator = new Mock<IAuthorizeResponseGenerator>();
            _mockEventService2 = new Mock<IEventService>();
        }

        [Fact]
        public async Task ProcessAuthorizeRequest_WhenInteractionGeneratorReturnsLogin_ReturnsLoginPage()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var user = new ClaimsPrincipal();
            var consent = new ConsentResponse();
            var validatedRequest = new ValidatedAuthorizeRequest();

            _mockResourceValidator.Setup(x => x.ValidateAsync(parameters, user))
                .ReturnsAsync(new AuthorizeRequestValidationResult(validatedRequest));

            _mockInteractionService.Setup(x => x.ProcessInteractionAsync(validatedRequest, consent))
                .ReturnsAsync(new InteractionResponse { IsLogin = true });

            var endpoint = new AuthorizeEndpointBase(
                _mockEventService.Object,
                _mockLogger.Object,
                _mockInteractionService.Object,
                _mockResourceValidator.Object,
                _mockConsentService.Object,
                _mockProfileService.Object,
                _mockAuthorizeResponseGenerator.Object,
                _mockEventService2.Object);

            // Act
            var result = await endpoint.ProcessRequestAsync(parameters, user, consent);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ProcessAuthorizeRequest_WhenInteractionGeneratorReturnsLogin_ReturnsLoginPage()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var user = new ClaimsPrincipal();
            var consent = new ConsentResponse();
            var validatedRequest = new ValidatedAuthorizeRequest();
            var authorizeResponse = new AuthorizeResponse { Request = validatedRequest };

            _mockResourceValidator.Setup(x => x.ValidateAsync(parameters, user))
                .ReturnsAsync(new AuthorizeRequestValidationResult(validatedRequest));

            _mockInteractionService.Setup(x => x.ProcessInteractionAsync(validatedRequest, consent))
                .ReturnsAsync(new InteractionResponse());

            _mockAuthorizeResponseGenerator.Setup(x => x.CreateResponseAsync(validatedRequest))
                .ReturnsAsync(authorizeResponse);

            var endpoint = new AuthorizeEndpointBase(
                _mockEventService.Object,
                _mockLogger.Object,
                _mockInteractionService.Object,
                _mockResourceValidator.Object,
                _mockConsentService.Object,
                _mockProfileService.Object,
                _mockAuthorizeResponseGenerator.Object,
                _mockEventService2.Object);

            // Act
            var result = await endpoint.ProcessRequestAsync(parameters, user, consent);

            // Assert
            Assert.NotNull(result);
            var authorizeResult = Assert.IsType<AuthorizeResult>(result);
            Assert.Same(authorizeResponse, authorizeResult.Response);
        }
    }
}
