using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class IUserSessionExtensionsTests
    {
        private readonly MockUserSession _mockUserSession = new MockUserSession();
        private const string SubjectId = "123";
        private const string SessionId = "456";

        public IUserSessionExtensionsTests()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", SubjectId)
            }));
            
            _mockUserSession.User = user;
            _mockUserSession.SessionId = SessionId;
        }

        [Fact]
        public async Task GetLogoutNotificationContext_WhenClientsExist_ShouldReturnContext()
        {
            // Arrange
            var clientIds = new[] { "client1", "client2" };
            _mockUserSession.Clients = clientIds;

            // Act
            var result = await _mockUserSession.GetLogoutNotificationContext();

            // Assert
            result.Should().NotBeNull();
            result.SubjectId.Should().Be(SubjectId);
            result.SessionId.Should().Be(SessionId);
            result.ClientIds.Should().BeEquivalentTo(clientIds);
        }

        [Fact]
        public async Task GetLogoutNotificationContext_WhenNoClients_ShouldReturnNull()
        {
            // Arrange
            _mockUserSession.Clients = new string[] { };

            // Act
            var result = await _mockUserSession.GetLogoutNotificationContext();

            // Assert
            result.Should().BeNull();
        }
    }
}
