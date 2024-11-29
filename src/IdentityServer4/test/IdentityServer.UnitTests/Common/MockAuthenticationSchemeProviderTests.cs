using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockAuthenticationSchemeProviderTests
    {
        private MockAuthenticationSchemeProvider _provider;

        public MockAuthenticationSchemeProviderTests()
        {
            _provider = new MockAuthenticationSchemeProvider();
        }

        [Fact]
        public async Task GetSchemeAsync_ShouldReturnScheme_WhenSchemeExists()
        {
            // Arrange
            var expectedScheme = new AuthenticationScheme("test", null, typeof(MockAuthenticationHandler));
            _provider.AddScheme(expectedScheme);

            // Act
            var result = await _provider.GetSchemeAsync("test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test", result.Name);
            Assert.Equal(typeof(MockAuthenticationHandler), result.HandlerType);
        }

        [Fact]
        public async Task GetSchemeAsync_ShouldReturnNull_WhenSchemeDoesNotExist()
        {
            // Act
            var result = await _provider.GetSchemeAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveScheme_ShouldRemoveExistingScheme()
        {
            // Arrange
            var scheme = new AuthenticationScheme("test", null, typeof(MockAuthenticationHandler));
            _provider.AddScheme(scheme);

            // Act
            _provider.RemoveScheme("test");
            var result = await _provider.GetSchemeAsync("test");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllSchemesAsync_ShouldReturnAllSchemes()
        {
            // Arrange
            var scheme1 = new AuthenticationScheme("test1", null, typeof(MockAuthenticationHandler));
            var scheme2 = new AuthenticationScheme("test2", null, typeof(MockAuthenticationHandler));
            _provider.AddScheme(scheme1);
            _provider.AddScheme(scheme2);

            // Act
            var result = await _provider.GetAllSchemesAsync();

            // Assert
            Assert.Equal(3, result.Count()); // Including default scheme
            Assert.Contains(result, s => s.Name == "test1");
            Assert.Contains(result, s => s.Name == "test2");
        }

        [Fact]
        public async Task GetDefaultAuthenticateSchemeAsync_ShouldReturnDefaultScheme()
        {
            // Act
            var result = await _provider.GetDefaultAuthenticateSchemeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("scheme", result.Name);
        }

        [Fact]
        public async Task DefaultSchemeProperties_ShouldReturnSameScheme()
        {
            // Act
            var authenticateScheme = await _provider.GetDefaultAuthenticateSchemeAsync();
            var challengeScheme = await _provider.GetDefaultChallengeSchemeAsync();
            var forbidScheme = await _provider.GetDefaultForbidSchemeAsync();
            var signInScheme = await _provider.GetDefaultSignInSchemeAsync();
            var signOutScheme = await _provider.GetDefaultSignOutSchemeAsync();

            // Assert
            Assert.Equal(authenticateScheme, challengeScheme);
            Assert.Equal(authenticateScheme, forbidScheme);
            Assert.Equal(authenticateScheme, signInScheme);
            Assert.Equal(authenticateScheme, signOutScheme);
        }
    }
}
