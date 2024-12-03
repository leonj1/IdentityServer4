using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class SecretValidationExtendedTests
    {
        private const string Category = "Secrets - Secret Validator Extended";

        private ISecretValidator _hashedSecretValidator;
        private IClientStore _clients;
        private SecretValidator _validator;
        private IdentityServerOptions _options;

        public SecretValidationExtendedTests()
        {
            _hashedSecretValidator = new HashedSharedSecretValidator(new Logger<HashedSharedSecretValidator>(new LoggerFactory()));
            _clients = new InMemoryClientStore(ClientValidationTestClients.Get());
            _options = new IdentityServerOptions();
            _validator = new SecretValidator(
                new StubClock(),
                new[] { _hashedSecretValidator },
                new Logger<SecretValidator>(new LoggerFactory()));
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Null_Secret_Should_Fail()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var result = await _validator.ValidateAsync(client.ClientSecrets, null);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Secret_Collection_Should_Fail()
        {
            var secret = new ParsedSecret
            {
                Id = "test_client",
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(new List<Secret>(), secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Null_Secret_Collection_Should_Fail()
        {
            var secret = new ParsedSecret
            {
                Id = "test_client",
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(null, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Credential_Should_Fail()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Credential_Should_Fail()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = string.Empty,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }
    }
}
