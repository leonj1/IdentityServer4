using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class AuthorizeInteractionResponseGeneratorTests
    {
        private readonly AuthorizeInteractionResponseGenerator _subject;
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<ILogger<AuthorizeInteractionResponseGenerator>> _logger;
        private readonly Mock<IConsentService> _consent;
        private readonly Mock<IProfileService> _profile;
        
        public AuthorizeInteractionResponseGeneratorTests()
        {
            _clock = new Mock<ISystemClock>();
            _logger = new Mock<ILogger<AuthorizeInteractionResponseGenerator>>();
            _consent = new Mock<IConsentService>();
            _profile = new Mock<IProfileService>();
            
            _subject = new AuthorizeInteractionResponseGenerator(
                _clock.Object,
                _logger.Object,
                _consent.Object,
                _profile.Object);
        }

        [Fact]
        public async Task ProcessInteraction_WithNullRequest_ShouldThrowArgumentNullException()
        {
            // Act
            Func<Task> act = () => _subject.ProcessInteractionAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessLoginAsync_WhenPromptIsLogin_ShouldReturnLoginResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                PromptModes = new[] { OidcConstants.PromptModes.Login }
            };

            // Act
            var result = await _subject.ProcessInteractionAsync(request);

            // Assert
            result.IsLogin.Should().BeTrue();
        }

        [Fact]
        public async Task ProcessLoginAsync_WhenUserNotAuthenticated_ShouldReturnLoginResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            // Act
            var result = await _subject.ProcessInteractionAsync(request);

            // Assert
            result.IsLogin.Should().BeTrue();
        }

        [Fact]
        public async Task ProcessConsentAsync_WhenPromptIsConsent_ShouldReturnConsentResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "123") }, "test")),
                PromptModes = new[] { OidcConstants.PromptModes.Consent },
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult()
            };

            _consent.Setup(x => x.RequiresConsentAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<Client>(), It.IsAny<ParsedScopeValue[]>()))
                .ReturnsAsync(true);

            // Act
            var result = await _subject.ProcessInteractionAsync(request);

            // Assert
            result.IsConsent.Should().BeTrue();
        }

        [Fact]
        public async Task ProcessConsentAsync_WhenConsentGranted_ShouldReturnSuccessResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "123") }, "test")),
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult()
            };

            var consent = new ConsentResponse
            {
                Granted = true,
                ScopesValuesConsented = new[] { "openid", "profile" }
            };

            // Act
            var result = await _subject.ProcessInteractionAsync(request, consent);

            // Assert
            result.IsConsent.Should().BeFalse();
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task ProcessConsentAsync_WhenConsentDenied_ShouldReturnAccessDeniedError()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "123") }, "test")),
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult()
            };

            var consent = new ConsentResponse
            {
                Granted = false,
                Error = AuthorizationError.AccessDenied
            };

            // Act
            var result = await _subject.ProcessInteractionAsync(request, consent);

            // Assert
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.AccessDenied);
        }
    }
}
