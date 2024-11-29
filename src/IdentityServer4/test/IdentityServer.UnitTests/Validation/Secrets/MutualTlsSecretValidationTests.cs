using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class MutualTlsSecretValidationTests
    {
        private const string Category = "Secrets - MutualTls Secret Validation Additional Tests";
        private IClientStore _clients = new InMemoryClientStore(ClientValidationTestClients.Get());

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_Null_Client_Should_Fail()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var secret = new ParsedSecret
            {
                Id = "unknown_client",
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(null, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_Null_Client_Should_Fail()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var secret = new ParsedSecret
            {
                Id = "unknown_client",
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(null, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_Null_Secret_Should_Fail()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_valid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var result = await validator.ValidateAsync(client.ClientSecrets, null);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_Null_Secret_Should_Fail()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_valid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var result = await validator.ValidateAsync(client.ClientSecrets, null);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Client_Secrets_Should_Fail()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var secret = new ParsedSecret
            {
                Id = "client",
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(new Secret[] { }, secret);
            result.Success.Should().BeFalse();
        }
    }
}
