using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultKeyMaterialServiceTests
    {
        private readonly Mock<ISigningCredentialStore> _signingCredentialStore;
        private readonly Mock<IValidationKeysStore> _validationKeysStore;
        private readonly DefaultKeyMaterialService _subject;

        public DefaultKeyMaterialServiceTests()
        {
            _signingCredentialStore = new Mock<ISigningCredentialStore>();
            _validationKeysStore = new Mock<IValidationKeysStore>();
            
            _subject = new DefaultKeyMaterialService(
                new[] { _validationKeysStore.Object },
                new[] { _signingCredentialStore.Object }
            );
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_WhenNoAllowedAlgorithms_ShouldReturnCredentials()
        {
            // Arrange
            var expectedCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()), "RS256");
            _signingCredentialStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync(expectedCredentials);

            // Act
            var result = await _subject.GetSigningCredentialsAsync();

            // Assert
            result.Should().BeSameAs(expectedCredentials);
            _signingCredentialStore.Verify(x => x.GetSigningCredentialsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_WhenAllowedAlgorithmsSpecified_ShouldReturnMatchingCredentials()
        {
            // Arrange
            var expectedCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()), "RS256");
            _signingCredentialStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync(expectedCredentials);

            // Act
            var result = await _subject.GetSigningCredentialsAsync(new[] { "RS256" });

            // Assert
            result.Should().BeSameAs(expectedCredentials);
            _signingCredentialStore.Verify(x => x.GetSigningCredentialsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_WhenAlgorithmNotAllowed_ShouldThrowException()
        {
            // Arrange
            var credentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()), "RS256");
            _signingCredentialStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync(credentials);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _subject.GetSigningCredentialsAsync(new[] { "ES256" }));
        }

        [Fact]
        public async Task GetValidationKeysAsync_ShouldReturnKeysFromAllStores()
        {
            // Arrange
            var expectedKeys = new[]
            {
                new SecurityKeyInfo { Key = new RsaSecurityKey(RSA.Create()) }
            };
            
            _validationKeysStore.Setup(x => x.GetValidationKeysAsync())
                .ReturnsAsync(expectedKeys);

            // Act
            var result = await _subject.GetValidationKeysAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedKeys);
            _validationKeysStore.Verify(x => x.GetValidationKeysAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllSigningCredentialsAsync_ShouldReturnCredentialsFromAllStores()
        {
            // Arrange
            var expectedCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()), "RS256");
            _signingCredentialStore.Setup(x => x.GetSigningCredentialsAsync())
                .ReturnsAsync(expectedCredentials);

            // Act
            var result = await _subject.GetAllSigningCredentialsAsync();

            // Assert
            result.Should().ContainSingle();
            result.First().Should().BeSameAs(expectedCredentials);
            _signingCredentialStore.Verify(x => x.GetSigningCredentialsAsync(), Times.Once);
        }
    }
}
