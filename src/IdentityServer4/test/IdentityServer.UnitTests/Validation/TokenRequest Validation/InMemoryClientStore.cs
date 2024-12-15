using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class InMemoryClientStore : IClientStore
    {
        private readonly List<Client> _clients;

        public InMemoryClientStore(List<Client> clients)
        {
            _clients = clients;
        }

        public Task<Client> FindEnabledClientByIdAsync(string clientId)
        {
            var client = _clients.Find(c => c.ClientId == clientId && !c.Enabled);
            return Task.FromResult(client);
        }
    }
}
