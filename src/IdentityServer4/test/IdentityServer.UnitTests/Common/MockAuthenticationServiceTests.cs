using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockAuthenticationServiceTests
    {
        private readonly MockAuthenticationService _sut;
        private readonly HttpContext _httpContext;

        public MockAuthenticationServiceTests()
        {
            _sut = new MockAuthenticationService();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnConfiguredResult()
        {
            // Arrange
            var expectedResult = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), "TestScheme"));
            _sut.Result = expectedResult;

            // Act
            var result = await _sut.AuthenticateAsync(_httpContext, "TestScheme");

            // Assert
            Assert.Same(expectedResult, result);
        }

        [Fact]
        public async Task ChallengeAsync_ShouldComplete()
        {
            // Act
            await _sut.ChallengeAsync(_httpContext, "TestScheme", new AuthenticationProperties());

            // Assert - no exception means success
            Assert.True(true);
        }

        [Fact]
        public async Task ForbidAsync_ShouldComplete()
        {
            // Act
            await _sut.ForbidAsync(_httpContext, "TestScheme", new AuthenticationProperties());

            // Assert - no exception means success
            Assert.True(true);
        }

        [Fact]
        public async Task SignInAsync_ShouldComplete()
        {
            // Arrange
            var principal = new ClaimsPrincipal();

            // Act
            await _sut.SignInAsync(_httpContext, "TestScheme", principal, new AuthenticationProperties());

            // Assert - no exception means success
            Assert.True(true);
        }

        [Fact]
        public async Task SignOutAsync_ShouldComplete()
        {
            // Act
            await _sut.SignOutAsync(_httpContext, "TestScheme", new AuthenticationProperties());

            // Assert - no exception means success
            Assert.True(true);
        }
    }
}
