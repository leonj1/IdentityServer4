using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IdentityModel;

namespace IdentityServer4.UnitTests.ResponseHandling
{
    public class AuthorizeInteractionResponseGeneratorTests
    {
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<ILogger<AuthorizeInteractionResponseGenerator>> _logger;
        private readonly Mock<IConsentService> _consent;
        private readonly Mock<IProfileService> _profile;
        private readonly AuthorizeInteractionResponseGenerator _subject;

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
        public async Task ProcessLoginAsync_WhenPromptModeIsLogin_ShouldReturnLoginResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                PromptModes = new[] { OidcConstants.PromptModes.Login },
                Client = new Client(),
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            // Act
            var result = await _subject.ProcessLoginAsync(request);

            // Assert
            Assert.True(result.IsLogin);
        }

        [Fact]
        public async Task ProcessLoginAsync_WhenUserNotAuthenticated_ShouldReturnLoginResponse()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                Client = new Client(),
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            // Act
            var result = await _subject.ProcessLoginAsync(request);

            // Assert
            Assert.True(result.IsLogin);
        }

        [Fact]
        public async Task ProcessConsentAsync_WhenPromptModeIsConsent_ShouldRequireConsent()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                PromptModes = new[] { OidcConstants.PromptModes.Consent },
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult(),
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            // Act
            var result = await _subject.ProcessConsentAsync(request);

            // Assert
            Assert.True(result.IsConsent);
        }

        [Fact]
        public async Task ProcessConsentAsync_WhenConsentRequired_AndPromptModeIsNone_ShouldReturnError()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest
            {
                PromptModes = new[] { OidcConstants.PromptModes.None },
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult(),
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            _consent.Setup(x => x.RequiresConsentAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<Client>(), It.IsAny<IEnumerable<ParsedScopeValue>>()))
                   .ReturnsAsync(true);

            // Act
            var result = await _subject.ProcessConsentAsync(request);

            // Assert
            Assert.Equal(OidcConstants.AuthorizeErrors.ConsentRequired, result.Error);
        }
    }
}
