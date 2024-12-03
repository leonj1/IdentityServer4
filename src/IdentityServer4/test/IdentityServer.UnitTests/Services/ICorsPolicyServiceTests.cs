using System.Threading.Tasks;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class ICorsPolicyServiceTests
    {
        private class TestCorsPolicyService : ICorsPolicyService
        {
            private readonly bool _shouldAllow;

            public TestCorsPolicyService(bool shouldAllow)
            {
                _shouldAllow = shouldAllow;
            }

            public Task<bool> IsOriginAllowedAsync(string origin)
            {
                return Task.FromResult(_shouldAllow);
            }
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenAllowed_ReturnsTrue()
        {
            // Arrange
            var service = new TestCorsPolicyService(shouldAllow: true);
            var testOrigin = "https://test.com";

            // Act
            var result = await service.IsOriginAllowedAsync(testOrigin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenNotAllowed_ReturnsFalse()
        {
            // Arrange
            var service = new TestCorsPolicyService(shouldAllow: false);
            var testOrigin = "https://test.com";

            // Act
            var result = await service.IsOriginAllowedAsync(testOrigin);

            // Assert
            Assert.False(result);
        }
    }
}
