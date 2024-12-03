using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class DiscoveryResponseGeneratorTests
    {
        private readonly IdentityServerOptions _options;
        private readonly IResourceStore _resourceStore;
        private readonly IKeyMaterialService _keys;
        private readonly ExtensionGrantValidator _extensionGrants;
        private readonly ISecretsListParser _secretParsers;
        private readonly IResourceOwnerPasswordValidator _resourceOwnerValidator;
        private readonly ILogger<DiscoveryResponseGenerator> _logger;
        private readonly DiscoveryResponseGenerator _subject;

        public DiscoveryResponseGeneratorTests()
        {
            _options = new IdentityServerOptions();
            _resourceStore = new InMemoryResourcesStore(new List<IdentityResource>(), new List<ApiResource>(), new List<ApiScope>());
            _keys = new DefaultKeyMaterialService(new[] { new SigningCredentials(new RsaSecurityKey(RSA.Create()), "RS256") });
            _extensionGrants = new ExtensionGrantValidator(new List<IExtensionGrantValidator>());
            _secretParsers = new SecretParser(new List<ISecretParser>());
            _resourceOwnerValidator = new NotSupportedResourceOwnerPasswordValidator();
            _logger = new LoggerFactory().CreateLogger<DiscoveryResponseGenerator>();

            _subject = new DiscoveryResponseGenerator(
                _options,
                _resourceStore,
                _keys,
                _extensionGrants,
                _secretParsers,
                _resourceOwnerValidator,
                _logger);
        }

        [Fact]
        public async Task CreateDiscoveryDocument_Should_Return_Basic_Document()
        {
            // Arrange
            var baseUrl = "https://localhost:5001";
            var issuerUri = baseUrl;

            // Act
            var result = await _subject.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey(OidcConstants.Discovery.Issuer);
            result[OidcConstants.Discovery.Issuer].Should().Be(issuerUri);
        }

        [Fact]
        public async Task CreateDiscoveryDocument_Should_Include_Endpoints_When_Enabled()
        {
            // Arrange
            var baseUrl = "https://localhost:5001";
            var issuerUri = baseUrl;
            _options.Discovery.ShowEndpoints = true;
            _options.Endpoints.EnableTokenEndpoint = true;
            _options.Endpoints.EnableAuthorizeEndpoint = true;

            // Act
            var result = await _subject.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

            // Assert
            result.Should().ContainKey(OidcConstants.Discovery.TokenEndpoint);
            result.Should().ContainKey(OidcConstants.Discovery.AuthorizationEndpoint);
            result[OidcConstants.Discovery.TokenEndpoint].Should().Be(baseUrl + Constants.ProtocolRoutePaths.Token);
            result[OidcConstants.Discovery.AuthorizationEndpoint].Should().Be(baseUrl + Constants.ProtocolRoutePaths.Authorize);
        }

        [Fact]
        public async Task CreateJwkDocument_Should_Return_Keys()
        {
            // Act
            var result = await _subject.CreateJwkDocumentAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.First().kty.Should().Be("RSA");
            result.First().use.Should().Be("sig");
        }
    }
}
