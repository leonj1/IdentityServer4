using System;
using System.Linq;
using Xunit;
using IdentityServer4.Models;
using IdentityServer.UnitTests.Common;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class ClientValidationTestClientsTests
    {
        [Fact]
        public void Get_Should_Return_Expected_Number_Of_Clients()
        {
            var clients = ClientValidationTestClients.Get();
            Assert.Equal(12, clients.Count);
        }

        [Fact]
        public void Disabled_Client_Should_Be_Configured_Correctly()
        {
            var clients = ClientValidationTestClients.Get();
            var client = clients.Single(x => x.ClientId == "disabled_client");
            
            Assert.False(client.Enabled);
            Assert.Single(client.ClientSecrets);
            Assert.Equal("secret", client.ClientSecrets.First().Value);
        }

        [Fact]
        public void Certificate_Valid_Client_Should_Have_Valid_Thumbprint()
        {
            var clients = ClientValidationTestClients.Get();
            var client = clients.Single(x => x.ClientId == "certificate_valid");
            
            Assert.True(client.Enabled);
            Assert.Single(client.ClientSecrets);
            Assert.Equal(IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateThumbprint, 
                client.ClientSecrets.First().Type);
            Assert.Equal(TestCert.Load().Thumbprint, client.ClientSecrets.First().Value);
        }

        [Fact]
        public void Multiple_Secrets_Hashed_Client_Should_Have_Expected_Secrets()
        {
            var clients = ClientValidationTestClients.Get();
            var client = clients.Single(x => x.ClientId == "multiple_secrets_hashed");
            
            Assert.Equal(5, client.ClientSecrets.Count);
            Assert.Contains(client.ClientSecrets, s => s.Description == "some description");
            
            // Verify expired secret
            var expiredSecret = client.ClientSecrets.Last();
            Assert.True(expiredSecret.Expiration < DateTime.UtcNow);
        }

        [Fact]
        public void MTLS_Valid_Client_Should_Have_Correct_Certificate_Configuration()
        {
            var clients = ClientValidationTestClients.Get();
            var client = clients.Single(x => x.ClientId == "mtls_client_valid");
            
            Assert.Equal(2, client.ClientSecrets.Count);
            Assert.Contains(client.ClientSecrets, 
                s => s.Type == IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateName);
            Assert.Contains(client.ClientSecrets, 
                s => s.Type == IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateThumbprint);
        }
    }
}
