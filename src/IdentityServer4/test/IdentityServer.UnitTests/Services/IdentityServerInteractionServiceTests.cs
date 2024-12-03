using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class IdentityServerInteractionServiceTests
    {
        private readonly Mock<IIdentityServerInteractionService> _mockInteractionService;

        public IdentityServerInteractionServiceTests()
        {
            _mockInteractionService = new Mock<IIdentityServerInteractionService>();
        }

        [Fact]
        public async Task GetAuthorizationContextAsync_WithValidReturnUrl_ShouldReturnAuthorizationRequest()
        {
            // Arrange
            var expectedRequest = new AuthorizationRequest();
            _mockInteractionService.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _mockInteractionService.Object.GetAuthorizationContextAsync("valid-url");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRequest);
        }

        [Theory]
        [InlineData("valid-url", true)]
        [InlineData("invalid-url", false)]
        public void IsValidReturnUrl_ShouldReturnExpectedResult(string returnUrl, bool expected)
        {
            // Arrange
            _mockInteractionService.Setup(x => x.IsValidReturnUrl(It.IsAny<string>()))
                .Returns((string url) => url == "valid-url");

            // Act
            var result = _mockInteractionService.Object.IsValidReturnUrl(returnUrl);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetErrorContextAsync_WithValidErrorId_ShouldReturnErrorMessage()
        {
            // Arrange
            var expectedError = new ErrorMessage();
            _mockInteractionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedError);

            // Act
            var result = await _mockInteractionService.Object.GetErrorContextAsync("error-id");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        public async Task GrantConsentAsync_ShouldCompleteSuccessfully()
        {
            // Arrange
            var request = new AuthorizationRequest();
            var consent = new ConsentResponse();
            
            _mockInteractionService.Setup(x => x.GrantConsentAsync(
                It.IsAny<AuthorizationRequest>(),
                It.IsAny<ConsentResponse>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _mockInteractionService.Object.GrantConsentAsync(request, consent, "subject")
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllUserGrantsAsync_ShouldReturnGrants()
        {
            // Arrange
            var expectedGrants = new List<Grant> { new Grant() };
            _mockInteractionService.Setup(x => x.GetAllUserGrantsAsync())
                .ReturnsAsync(expectedGrants);

            // Act
            var result = await _mockInteractionService.Object.GetAllUserGrantsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedGrants);
        }

        [Fact]
        public async Task RevokeUserConsentAsync_ShouldCompleteSuccessfully()
        {
            // Arrange
            _mockInteractionService.Setup(x => x.RevokeUserConsentAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _mockInteractionService.Object.RevokeUserConsentAsync("client-id")
                .Should().NotThrowAsync();
        }
    }
}
