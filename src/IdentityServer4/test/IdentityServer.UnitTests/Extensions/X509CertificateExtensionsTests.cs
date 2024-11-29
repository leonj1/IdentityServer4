using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class X509CertificateExtensionsTests : IDisposable
    {
        private X509Certificate2 _testCert;

        public X509CertificateExtensionsTests()
        {
            // Create a test certificate for each test
            using (var algorithm = RSA.Create(2048))
            {
                var request = new CertificateRequest(
                    "CN=Test Certificate",
                    algorithm,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                _testCert = request.CreateSelfSigned(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddYears(1));
            }
        }

        public void Dispose()
        {
            _testCert?.Dispose();
        }

        [Fact]
        public void CreateThumbprintCnf_Should_Create_Valid_Cnf_Value()
        {
            // Act
            var cnf = _testCert.CreateThumbprintCnf();

            // Assert
            var expectedHash = _testCert.GetCertHash(HashAlgorithmName.SHA256);
            var expectedValue = Base64Url.Encode(expectedHash);

            var json = JsonSerializer.Deserialize<JsonDocument>(cnf);
            json.RootElement.GetProperty("x5t#S256").GetString()
                .Should().Be(expectedValue);
        }
    }
}
