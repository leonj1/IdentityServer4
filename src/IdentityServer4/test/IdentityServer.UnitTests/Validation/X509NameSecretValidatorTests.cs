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

namespace IdentityServer.UnitTests.Validation
{
    public class X509NameSecretValidatorTests
    {
        private readonly ILogger<X509NameSecretValidator> _logger;
        private readonly X509NameSecretValidator _validator;

        public X509NameSecretValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<X509NameSecretValidator>();
            _validator = new X509NameSecretValidator(_logger);
        }

        [Fact]
        public async Task Invalid_Secret_Type_Should_Fail()
        {
            // Arrange
            var secrets = new[] { new Secret("secret") };
            var parsedSecret = new ParsedSecret
            {
                Type = "invalid",
                Credential = "secret"
            };

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Valid_Certificate_With_Matching_Name_Should_Succeed()
        {
            // Arrange
            var cert = new X509Certificate2("test.pfx", "password");
            var secrets = new[] 
            { 
                new Secret(cert.Subject, SecretTypes.X509CertificateName) 
            };
            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = cert
            };

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
            result.Confirmation.Should().NotBeNull();
        }

        [Fact]
        public async Task Certificate_With_No_Subject_Should_Fail()
        {
            // Arrange
            var cert = new X509Certificate2(); // Empty cert
            var secrets = new[] 
            { 
                new Secret("CN=test", SecretTypes.X509CertificateName) 
            };
            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = cert
            };

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Non_Matching_Certificate_Name_Should_Fail()
        {
            // Arrange
            var cert = new X509Certificate2("test.pfx", "password");
            var secrets = new[] 
            { 
                new Secret("CN=different", SecretTypes.X509CertificateName) 
            };
            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = cert
            };

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public void Invalid_Credential_Type_Should_Throw()
        {
            // Arrange
            var secrets = new[] 
            { 
                new Secret("secret", SecretTypes.X509CertificateName) 
            };
            var parsedSecret = new ParsedSecret
            {
                Type = ParsedSecretTypes.X509Certificate,
                Credential = "not a certificate"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => 
                _validator.ValidateAsync(secrets, parsedSecret));
        }
    }
}
