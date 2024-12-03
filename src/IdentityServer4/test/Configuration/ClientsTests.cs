using IdentityServer4.Models;
using IdentityServerHost.Configuration;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class ClientsTests
    {
        [Fact]
        public void Get_Should_Return_Combined_Clients()
        {
            // Act
            var result = Clients.Get().ToList();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            
            // Verify we have clients from both sources
            var consoleClients = ClientsConsole.Get().ToList();
            var webClients = ClientsWeb.Get().ToList();
            
            Assert.Equal(consoleClients.Count + webClients.Count, result.Count);
            
            // Verify all console clients are present
            foreach(var consoleClient in consoleClients)
            {
                Assert.Contains(result, c => c.ClientId == consoleClient.ClientId);
            }
            
            // Verify all web clients are present
            foreach(var webClient in webClients)
            {
                Assert.Contains(result, c => c.ClientId == webClient.ClientId);
            }
        }
    }
}
