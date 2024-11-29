using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class IRedirectUriValidatorTests
    {
        private class TestRedirectUriValidator : IRedirectUriValidator
        {
            public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
            {
                return Task.FromResult(requestedUri == "https://valid.example.com");
            }

            public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
            {
                return Task.FromResult(requestedUri == "https://valid-logout.example.com");
            }
        }

        private readonly IRedirectUriValidator _validator;
        private readonly Client _client;

        public IRedirectUriValidatorTests()
        {
            _validator = new TestRedirectUriValidator();
            _client = new Client
            {
                ClientId = "test_client"
            };
        }

        [Fact]
        public async Task IsRedirectUriValidAsync_WithValidUri_ReturnsTrue()
        {
            // Arrange
            var validUri = "https://valid.example.com";

            // Act
            var result = await _validator.IsRedirectUriValidAsync(validUri, _client);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsRedirectUriValidAsync_WithInvalidUri_ReturnsFalse()
        {
            // Arrange
            var invalidUri = "https://invalid.example.com";

            // Act
            var result = await _validator.IsRedirectUriValidAsync(invalidUri, _client);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsPostLogoutRedirectUriValidAsync_WithValidUri_ReturnsTrue()
        {
            // Arrange
            var validUri = "https://valid-logout.example.com";

            // Act
            var result = await _validator.IsPostLogoutRedirectUriValidAsync(validUri, _client);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsPostLogoutRedirectUriValidAsync_WithInvalidUri_ReturnsFalse()
        {
            // Arrange
            var invalidUri = "https://invalid-logout.example.com";

            // Act
            var result = await _validator.IsPostLogoutRedirectUriValidAsync(invalidUri, _client);

            // Assert
            result.Should().BeFalse();
        }
    }
}
