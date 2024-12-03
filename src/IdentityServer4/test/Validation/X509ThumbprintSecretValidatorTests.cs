using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer4.UnitTests.Validation
{
    public class X509ThumbprintSecretValidatorTests
    {
        private readonly ILogger<X509ThumbprintSecretValidator> _logger = new LoggerFactory().CreateLogger<X509ThumbprintSecretValidator>();
        private readonly X509ThumbprintSecretValidator _validator;
        private readonly X509Certificate2 _cert;

        public X509ThumbprintSecretValidatorTests()
        {
            _validator = new X509ThumbprintSecretValidator(_logger);
            _cert = new X509Certificate2("testcert.pfx", "password");
        }

        [Fact]
        public async Task Valid_Thumbprint_Should_Success()
        {
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = SecretTypes.X509CertificateThumbprint,
                    Value = _cert.Thumbprint
                }
            };

            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = _cert
            };

            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            result.Success.Should().BeTrue();
            result.Confirmation.Should().NotBeNull();
        }

        [Fact]
        public async Task Invalid_Secret_Type_Should_Fail()
        {
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = SecretTypes.X509CertificateThumbprint,
                    Value = _cert.Thumbprint
                }
            };

            var parsedSecret = new ParsedSecret
            {
                Type = "invalid_type",
                Credential = _cert
            };

            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task No_Thumbprint_Secrets_Should_Fail()
        {
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = "invalid_type",
                    Value = "value"
                }
            };

            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = _cert
            };

            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Non_Matching_Thumbprint_Should_Fail()
        {
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = SecretTypes.X509CertificateThumbprint,
                    Value = "non_matching_thumbprint"
                }
            };

            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = _cert
            };

            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Invalid_Credential_Type_Should_Throw()
        {
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = SecretTypes.X509CertificateThumbprint,
                    Value = _cert.Thumbprint
                }
            };

            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = "invalid_credential"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _validator.ValidateAsync(secrets, parsedSecret));
        }
    }
}
