using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Stores;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Stores
{
    public class SigningCredentialStoreTests
    {
        private readonly Mock<ISigningCredentialStore> _mockStore;

        public SigningCredentialStoreTests()
        {
            _mockStore = new Mock<ISigningCredentialStore>();
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_ShouldReturnCredentials()
        {
            // Arrange
            var expectedCredentials = new SigningCredentials(
                new SymmetricSecurityKey(new byte[32]), 
                SecurityAlgorithms.HmacSha256);
            
            _mockStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync(expectedCredentials);

            // Act
            var result = await _mockStore.Object.GetSigningCredentialsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(expectedCredentials);
            _mockStore.Verify(x => x.GetSigningCredentialsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_WhenNoCredentials_ShouldReturnNull()
        {
            // Arrange
            _mockStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync((SigningCredentials)null);

            // Act
            var result = await _mockStore.Object.GetSigningCredentialsAsync();

            // Assert
            result.Should().BeNull();
            _mockStore.Verify(x => x.GetSigningCredentialsAsync(), Times.Once);
        }
    }
}
