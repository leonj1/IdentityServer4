using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockSessionIdServiceTests
    {
        private readonly MockSessionIdService _sut;

        public MockSessionIdServiceTests()
        {
            _sut = new MockSessionIdService();
        }

        [Fact]
        public void GetCookieName_ShouldReturnCorrectName()
        {
            var result = _sut.GetCookieName();
            Assert.Equal("sessionid", result);
        }

        [Fact]
        public void GetCookieValue_ShouldReturnSessionId()
        {
            var result = _sut.GetCookieValue();
            Assert.Equal("session_id", result);
        }

        [Fact]
        public async Task GetCurrentSessionIdAsync_ShouldReturnSessionId()
        {
            var result = await _sut.GetCurrentSessionIdAsync();
            Assert.Equal("session_id", result);
        }

        [Fact]
        public void RemoveCookie_ShouldSetFlagAndClearSessionId()
        {
            // Act
            _sut.RemoveCookie();

            // Assert
            Assert.True(_sut.RemoveCookieWasCalled);
            Assert.Null(_sut.SessionId);
        }

        [Fact]
        public async Task EnsureSessionCookieAsync_ShouldComplete()
        {
            await _sut.EnsureSessionCookieAsync();
            // Test passes if no exception is thrown
        }

        [Fact]
        public void CreateSessionId_ShouldNotThrowException()
        {
            _sut.CreateSessionId(null);
            // Test passes if no exception is thrown
        }
    }
}
