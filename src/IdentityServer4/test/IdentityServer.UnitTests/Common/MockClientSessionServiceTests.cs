using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockClientSessionServiceTests
    {
        private readonly MockClientSessionService _sut;

        public MockClientSessionServiceTests()
        {
            _sut = new MockClientSessionService();
        }

        [Fact]
        public async Task AddClientIdAsync_ShouldAddClientToList()
        {
            // Arrange
            var clientId = "test_client";

            // Act
            await _sut.AddClientIdAsync(clientId);

            // Assert
            _sut.Clients.Should().Contain(clientId);
            _sut.Clients.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetClientListAsync_ShouldReturnAllClients()
        {
            // Arrange
            var clientIds = new[] { "client1", "client2", "client3" };
            foreach (var clientId in clientIds)
            {
                await _sut.AddClientIdAsync(clientId);
            }

            // Act
            var result = await _sut.GetClientListAsync();

            // Assert
            result.Should().BeEquivalentTo(clientIds);
        }

        [Fact]
        public void GetClientListFromCookie_ShouldReturnAllClients()
        {
            // Arrange
            _sut.Clients.AddRange(new[] { "client1", "client2" });

            // Act
            var result = _sut.GetClientListFromCookie("any_sid");

            // Assert
            result.Should().BeEquivalentTo(_sut.Clients);
        }

        [Fact]
        public void RemoveCookie_ShouldClearClientsAndSetFlag()
        {
            // Arrange
            _sut.Clients.AddRange(new[] { "client1", "client2" });

            // Act
            _sut.RemoveCookie("any_sid");

            // Assert
            _sut.RemoveCookieWasCalled.Should().BeTrue();
            _sut.Clients.Should().BeEmpty();
        }

        [Fact]
        public async Task EnsureClientListCookieAsync_ShouldComplete()
        {
            // Arrange & Act
            await _sut.EnsureClientListCookieAsync("any_sid");

            // Assert
            // Method should complete without throwing any exceptions
        }
    }
}
