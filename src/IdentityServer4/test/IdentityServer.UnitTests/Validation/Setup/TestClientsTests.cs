using System.Linq;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestClientsTests
    {
        [Fact]
        public void Get_Should_Return_Expected_Number_Of_Clients()
        {
            var clients = TestClients.Get();
            clients.Should().NotBeNull();
            clients.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void Clients_Should_Have_Valid_Configuration()
        {
            var clients = TestClients.Get();
            
            foreach (var client in clients)
            {
                client.ClientId.Should().NotBeNullOrEmpty();
                client.AllowedGrantTypes.Should().NotBeEmpty();
                
                if (client.RequireClientSecret)
                {
                    client.ClientSecrets.Should().NotBeEmpty();
                }

                if (client.AllowedGrantTypes.Contains("authorization_code") || 
                    client.AllowedGrantTypes.Contains("hybrid"))
                {
                    client.RedirectUris.Should().NotBeEmpty();
                }
            }
        }

        [Fact]
        public void Should_Contain_Code_Client()
        {
            var clients = TestClients.Get();
            var codeClient = clients.FirstOrDefault(x => x.ClientId == "codeclient");
            
            codeClient.Should().NotBeNull();
            codeClient.AllowedGrantTypes.Should().Contain("authorization_code");
            codeClient.ClientSecrets.Should().NotBeEmpty();
            codeClient.RequirePkce.Should().BeFalse();
        }

        [Fact]
        public void Should_Contain_Client_Credentials_Client()
        {
            var clients = TestClients.Get();
            var client = clients.FirstOrDefault(x => x.ClientId == "client");
            
            client.Should().NotBeNull();
            client.AllowedGrantTypes.Should().Contain("client_credentials");
            client.ClientSecrets.Should().NotBeEmpty();
            client.AllowOfflineAccess.Should().BeTrue();
            client.AccessTokenType.Should().Be(AccessTokenType.Jwt);
        }
    }
}
