using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class PlainTextClientSecretValidationTests
    {
        private const string Category = "Secrets - Additional PlainText Shared Secret Validation Tests";

        private ISecretValidator _validator = new PlainTextSharedSecretValidator(new Logger<PlainTextSharedSecretValidator>(new LoggerFactory()));
        private IClientStore _clients = new InMemoryClientStore(ClientValidationTestClients.Get());

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Credential_Should_Fail()
        {
            var clientId = "single_secret_no_protection_no_expiration";
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

        [Fact]
        [Trait("Category", Category)]
        public async Task Whitespace_Credential_Should_Fail()
        {
            var clientId = "single_secret_no_protection_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "   ",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Null_Client_Should_Fail()
        {
            var secret = new ParsedSecret
            {
                Id = "unknown",
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(null, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Case_Sensitive_Secret_Comparison()
        {
            var clientId = "single_secret_no_protection_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "SECRET", // uppercase
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Secret_With_Extra_Spaces_Should_Fail()
        {
            var clientId = "single_secret_no_protection_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret ",  // extra space
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }
    }
}
