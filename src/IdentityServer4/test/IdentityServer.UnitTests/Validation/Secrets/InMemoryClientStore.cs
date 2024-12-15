using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer4.Stores
{
    public class InMemoryClientStore : IClientStore
    {
        private readonly List<Client> _clients;

        public InMemoryClientStore(List<Client> clients)
        {
            _clients = clients;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _clients.Find(c => c.ClientId == clientId);
            return Task.FromResult(client);
        }
    }
}
