using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services.Default
{
    public class OidcReturnUrlParserTests
    {
        private readonly Mock<IAuthorizeRequestValidator> _validator;
        private readonly Mock<IUserSession> _userSession;
        private readonly Mock<ILogger<OidcReturnUrlParser>> _logger;
        private readonly Mock<IAuthorizationParametersMessageStore> _messageStore;
        private readonly OidcReturnUrlParser _parser;

        public OidcReturnUrlParserTests()
        {
            _validator = new Mock<IAuthorizeRequestValidator>();
            _userSession = new Mock<IUserSession>();
            _logger = new Mock<ILogger<OidcReturnUrlParser>>();
            _messageStore = new Mock<IAuthorizationParametersMessageStore>();
            
            _parser = new OidcReturnUrlParser(
                _validator.Object,
                _userSession.Object,
                _logger.Object,
                _messageStore.Object);
        }

        [Theory]
        [InlineData("/connect/authorize", true)]
        [InlineData("/connect/authorize/callback", true)]
        [InlineData("/connect/authorize?param=value", true)]
        [InlineData("/connect/authorize/callback?param=value", true)]
        [InlineData("/invalid/path", false)]
        [InlineData("http://external.com/connect/authorize", false)]
        public void IsValidReturnUrl_ShouldValidateCorrectly(string returnUrl, bool expectedResult)
        {
            // Act
            var result = _parser.IsValidReturnUrl(returnUrl);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task ParseAsync_WithValidReturnUrl_ShouldReturnAuthorizationRequest()
        {
            // Arrange
            var returnUrl = "/connect/authorize?client_id=test&scope=openid";
            var parameters = new NameValueCollection();
            var validationResult = new AuthorizeRequestValidationResult(new ValidatedAuthorizeRequest());
            
            _validator
                .Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _parser.ParseAsync(returnUrl);

            // Assert
            result.Should().NotBeNull();
            _validator.Verify(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task ParseAsync_WithInvalidReturnUrl_ShouldReturnNull()
        {
            // Arrange
            var returnUrl = "http://external.com/connect/authorize";

            // Act
            var result = await _parser.ParseAsync(returnUrl);

            // Assert
            result.Should().BeNull();
            _validator.Verify(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()), Times.Never);
        }

        [Fact]
        public async Task ParseAsync_WithMessageStore_ShouldUseStoredParameters()
        {
            // Arrange
            var returnUrl = "/connect/authorize?id=123";
            var storedParameters = new NameValueCollection { { "client_id", "test" }, { "scope", "openid" } };
            var messageStoreData = new Message<NameValueCollection>(storedParameters);
            
            _messageStore
                .Setup(x => x.ReadAsync("123"))
                .ReturnsAsync(messageStoreData);

            _validator
                .Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new AuthorizeRequestValidationResult(new ValidatedAuthorizeRequest()));

            // Act
            var result = await _parser.ParseAsync(returnUrl);

            // Assert
            result.Should().NotBeNull();
            _messageStore.Verify(x => x.ReadAsync("123"), Times.Once);
        }
    }
}
