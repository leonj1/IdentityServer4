using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenValidationTests
{
    public class IdentityServerBuilder : IAsyncLifetime
    {
        private readonly List<Client> _clients = new List<Client>();
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

        public async Task InitializeAsync()
        {
            // Initialize clients and refresh tokens here
        }

        public async Task DisposeAsync()
        {
            // Dispose resources here
        }

        public IClientStore Clients => new InMemoryClientStore(_clients);
        public IRefreshTokenStore RefreshTokens => new InMemoryRefreshTokenStore(_refreshTokens);

        public TokenRequestValidator Validator => new TokenRequestValidator(
            clients: _host.Clients,
            refreshTokens: _host.RefreshTokens,
            profile: new TestProfileService(),
            clock: new Clock()
        );
    }
}
