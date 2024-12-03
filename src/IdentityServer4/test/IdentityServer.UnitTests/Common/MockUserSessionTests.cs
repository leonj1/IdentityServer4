using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockUserSessionTests
    {
        private MockUserSession _subject;

        public MockUserSessionTests()
        {
            _subject = new MockUserSession();
        }

        [Fact]
        public async Task CreateSessionId_ShouldSetUserAndReturnSessionId()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var props = new AuthenticationProperties();

            // Act
            var sessionId = await _subject.CreateSessionIdAsync(user, props);

            // Assert
            _subject.CreateSessionIdWasCalled.Should().BeTrue();
            _subject.User.Should().BeSameAs(user);
            sessionId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUser_ShouldReturnStoredUser()
        {
            // Arrange
            var expected = new ClaimsPrincipal(new ClaimsIdentity());
            _subject.User = expected;

            // Act
            var actual = await _subject.GetUserAsync();

            // Assert
            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task EnsureSessionIdCookie_ShouldSetFlag()
        {
            // Act
            await _subject.EnsureSessionIdCookieAsync();

            // Assert
            _subject.EnsureSessionIdCookieWasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task RemoveSessionIdCookie_ShouldSetFlag()
        {
            // Act
            await _subject.RemoveSessionIdCookieAsync();

            // Assert
            _subject.RemoveSessionIdCookieWasCalled.Should().BeTrue();
        }

        [Fact]
        public async Task AddClientId_ShouldAddToClientsList()
        {
            // Arrange
            var clientId = "client1";

            // Act
            await _subject.AddClientIdAsync(clientId);

            // Assert
            _subject.Clients.Should().Contain(clientId);
        }

        [Fact]
        public async Task GetClientList_ShouldReturnStoredClients()
        {
            // Arrange
            _subject.Clients.Add("client1");
            _subject.Clients.Add("client2");

            // Act
            var clients = await _subject.GetClientListAsync();

            // Assert
            clients.Should().BeEquivalentTo(_subject.Clients);
        }
    }
}
