using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class IdentityServerBuilderExtensionsCryptoTests
    {
        private IIdentityServerBuilder _builder;
        private IServiceCollection _services;

        public IdentityServerBuilderExtensionsCryptoTests()
        {
            _services = new ServiceCollection();
            _builder = new IdentityServerBuilder(_services);
        }

        [Fact]
        public void AddSigningCredential_WithValidRsaKey_ShouldAddCorrectServices()
        {
            // Arrange
            var rsaKey = new RsaSecurityKey(RSA.Create());

            // Act
            _builder.AddSigningCredential(rsaKey, IdentityServerConstants.RsaSigningAlgorithm.RS256);

            // Assert
            var provider = _services.BuildServiceProvider();
            var signingCredentialStore = provider.GetRequiredService<ISigningCredentialStore>();
            var validationKeysStore = provider.GetRequiredService<IValidationKeysStore>();
            
            signingCredentialStore.Should().NotBeNull();
            validationKeysStore.Should().NotBeNull();
        }

        [Fact]
        public void AddSigningCredential_WithInvalidKey_ShouldThrowException()
        {
            // Arrange
            var symmetricKey = new SymmetricSecurityKey(new byte[32]);
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            // Act & Assert
            Action act = () => _builder.AddSigningCredential(credentials);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Signing key is not asymmetric");
        }

        [Fact]
        public void AddDeveloperSigningCredential_ShouldCreateTemporaryKey()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), "tempkey.jwk");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            // Act
            _builder.AddDeveloperSigningCredential(true, tempFile);

            // Assert
            File.Exists(tempFile).Should().BeTrue();
            
            // Cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public void AddValidationKey_WithValidKey_ShouldAddValidationKeysStore()
        {
            // Arrange
            var rsaKey = new RsaSecurityKey(RSA.Create());
            
            // Act
            _builder.AddValidationKey(rsaKey);

            // Assert
            var provider = _services.BuildServiceProvider();
            var validationKeysStore = provider.GetRequiredService<IValidationKeysStore>();
            validationKeysStore.Should().NotBeNull();
        }

        [Fact]
        public void AddSigningCredential_WithRsaKeyFromFile_ShouldAddCorrectServices()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), "testkey.jwk");
            var rsa = RSA.Create(2048);
            var key = new RsaSecurityKey(rsa);
            var parameters = rsa.ExportParameters(true);
            
            File.WriteAllText(tempFile, System.Text.Json.JsonSerializer.Serialize(parameters));

            try
            {
                // Act
                _builder.AddSigningCredential(key, IdentityServerConstants.RsaSigningAlgorithm.RS256);

                // Assert
                var provider = _services.BuildServiceProvider();
                var signingCredentialStore = provider.GetRequiredService<ISigningCredentialStore>();
                var validationKeysStore = provider.GetRequiredService<IValidationKeysStore>();

                signingCredentialStore.Should().NotBeNull();
                validationKeysStore.Should().NotBeNull();
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
