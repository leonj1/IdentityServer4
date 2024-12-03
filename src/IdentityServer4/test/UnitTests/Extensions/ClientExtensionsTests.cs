using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class ClientExtensionsTests
    {
        [Fact]
        public void IsImplicitOnly_WhenClientIsNull_ReturnsFalse()
        {
            Client client = null;
            client.IsImplicitOnly().Should().BeFalse();
        }

        [Fact]
        public void IsImplicitOnly_WhenGrantTypesIsNull_ReturnsFalse()
        {
            var client = new Client
            {
                AllowedGrantTypes = null
            };
            client.IsImplicitOnly().Should().BeFalse();
        }

        [Fact]
        public void IsImplicitOnly_WhenImplicitGrantType_ReturnsTrue()
        {
            var client = new Client
            {
                AllowedGrantTypes = new List<string> { GrantType.Implicit }
            };
            client.IsImplicitOnly().Should().BeTrue();
        }

        [Fact]
        public void IsImplicitOnly_WhenMultipleGrantTypes_ReturnsFalse()
        {
            var client = new Client
            {
                AllowedGrantTypes = new List<string> { GrantType.Implicit, GrantType.ClientCredentials }
            };
            client.IsImplicitOnly().Should().BeFalse();
        }

        [Fact]
        public async Task GetKeysAsync_WhenNoSecrets_ReturnsEmptyList()
        {
            var secrets = new List<Secret>();
            var keys = await secrets.GetKeysAsync();
            keys.Should().BeEmpty();
        }

        [Fact]
        public async Task GetKeysAsync_WithX509Certificate_ReturnsSecurityKey()
        {
            // Create a self-signed certificate for testing
            var cert = CreateSelfSignedCertificate();
            var certBase64 = Convert.ToBase64String(cert.Export(X509ContentType.Cert));
            
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                    Value = certBase64
                }
            };

            var keys = await secrets.GetKeysAsync();
            
            keys.Should().HaveCount(1);
            keys.First().Should().BeOfType<X509SecurityKey>();
        }

        [Fact]
        public async Task GetKeysAsync_WithJsonWebKey_ReturnsSecurityKey()
        {
            var jwk = @"{""kty"":""RSA"",""e"":""AQAB"",""kid"":""test-key"",""n"":""test123""}";
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                    Value = jwk
                }
            };

            var keys = await secrets.GetKeysAsync();
            
            keys.Should().HaveCount(1);
            keys.First().Should().BeOfType<JsonWebKey>();
        }

        private X509Certificate2 CreateSelfSignedCertificate()
        {
            var certBytes = Convert.FromBase64String(
                "MIIDBTCCAfGgAwIBAgIQNQb+T2ncIrNA6cKvUA1GWTAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjIwMDAwWhcNMjAwMTIwMjIwMDAwWjAVMRMwEQYDVQQDEwppZHNydjN0ZXN0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqnTksBdxOiOlsmRNd+mMS2M3o1IDpK4uAr0T4/YqO3zYHAGAWTwsq4ms+NWynqY5HaB4EThNxuq2GWC5JKpO1YirOrwS97B5x9LJyHXPsdJcSikEI9BxOkl6WLQ0UzPxHdYTLpR4/O+0ILAlXw8NU4+jB4AP8Sn9YGYJ5w0fLw5YmWioXeWvocz1wHrZdJPxS8XnqHXwMUozVzQj+x6daOv5FmrHU1r9/bbp0a1GLv4BbTtSh4kMyz1hXylho0EvPg5p9YIKStbNAW9eNWvv5R8HN7PPei21AsUqxekK0oW9jnEdHewckToX7x5zULWKwwZIksll0XnVczVgy7fCFwIDAQABo1wwWjATBgNVHSUEDDAKBggrBgEFBQcDATBDBgNVHQEEPDA6gBDSFgDaV+Q2d2191r6A38tBoRQwEjEQMA4GA1UEAxMHRGV2Um9vdIIQLFk7exPNg41NRNaeNu0I9jAJBgUrDgMCHQUAA4IBAQBUnMSZxY5xosMEW6Mz4WEAjNoNv2QvqNmk23RMZGMgr516ROeWS5D3RlTNyU8FkstNCC4maDM3E0Bi4bbzW3AwrpbluqtcyMN3Pivqdxx+zKWKiORJqqLIvN8CT1fVPxxXb/e9GOdaR8eXSmB0PgNUhM4IjgNkwBbvWC9F/lzvwjlQgciR7d4GfXPYsE1vf8tmdQaY8/PtdAkExmbrb9MihdggSoGXlELrPA91Yce+fiRcKY3rQlNWVd4DOoJ/cPXsXwry8pWjNCo5JD8Q+RQ5yZEy7YPoifwemLhTdsBz3hlZr28oCGJ3kbnpW0xGvQb3VHSTVVbeei0CfXoW6iz1");
            return new X509Certificate2(certBytes);
        }
    }
}
