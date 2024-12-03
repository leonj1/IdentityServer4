using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Stores;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemorySigningCredentialsStoreTests
    {
        [Fact]
        public async Task GetSigningCredentialsAsync_Should_Return_Stored_Credentials()
        {
            // Arrange
            var key = new SymmetricSecurityKey(new byte[] { 1, 2, 3, 4, 5 });
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var store = new InMemorySigningCredentialsStore(credentials);

            // Act
            var result = await store.GetSigningCredentialsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(credentials);
            result.Key.Should().BeSameAs(key);
            result.Algorithm.Should().Be(SecurityAlgorithms.HmacSha256);
        }
    }
}
