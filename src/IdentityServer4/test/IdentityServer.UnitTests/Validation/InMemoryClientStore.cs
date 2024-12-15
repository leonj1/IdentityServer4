using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class InMemoryClientStore : IClientStore
    {
        private readonly List<Client> _clients = new List<Client>
        {
            new Client
            {
                ClientId = "codeclient",
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://identityserver4-test.local/signin-oidc" },
                PostLogoutRedirectUris = { "https://identityserver4-test.local/signout-callback-oidc" },
                AllowedScopes = { "openid", "profile", "api1" }
            }
        };

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_clients.Find(c => c.ClientId == clientId));
        }

        public Task<IEnumerable<Client>> FindEnabledClientsAsync()
        {
            return Task.FromResult((IEnumerable<Client>)_clients);
        }
    }
}
