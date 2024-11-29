using Xunit;
using System.Linq;
using IdentityServer4.Models;

namespace IdentityServerHost.Configuration.Tests
{
    public class ClientsTests
    {
        [Fact]
        public void Get_ShouldReturnNonEmptyClientCollection()
        {
            // Act
            var clients = Clients.Get();

            // Assert
            Assert.NotNull(clients);
            Assert.NotEmpty(clients);
        }

        [Fact]
        public void Get_ShouldIncludeConsoleAndWebClients()
        {
            // Act
            var allClients = Clients.Get().ToList();
            var consoleClients = ClientsConsole.Get().ToList();
            var webClients = ClientsWeb.Get().ToList();

            // Assert
            Assert.True(allClients.Count >= consoleClients.Count + webClients.Count);
            
            // Verify console clients are included
            foreach (var consoleClient in consoleClients)
            {
                Assert.Contains(allClients, c => c.ClientId == consoleClient.ClientId);
            }

            // Verify web clients are included
            foreach (var webClient in webClients)
            {
                Assert.Contains(allClients, c => c.ClientId == webClient.ClientId);
            }
        }
    }
}
