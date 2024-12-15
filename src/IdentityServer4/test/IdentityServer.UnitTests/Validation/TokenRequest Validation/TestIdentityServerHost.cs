using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenValidationTests
{
    public class TestIdentityServerHost : IAsyncLifetime
    {
        private readonly IdentityServerHost _host;

        public TestIdentityServerHost()
        {
            _host = new IdentityServerHost();
        }

        public async Task InitializeAsync()
        {
            await _host.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            await _host.DisposeAsync();
        }

        public IClientStore Clients => _host.Clients;
        public IRefreshTokenStore RefreshTokens => _host.RefreshTokens;

        public TokenRequestValidator Validator => new TokenRequestValidator(
            clients: _host.Clients,
            refreshTokens: _host.RefreshTokens,
            profile: new TestProfileService(),
            clock: new Clock()
        );
    }
}
