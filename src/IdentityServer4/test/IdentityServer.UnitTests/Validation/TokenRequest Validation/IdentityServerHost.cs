using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenValidationTests
{
    public class IdentityServerHost : IAsyncLifetime
    {
        private readonly IdentityServerBuilder _builder;

        public IdentityServerHost()
        {
            _builder = new IdentityServerBuilder();
        }

        public async Task InitializeAsync()
        {
            await _builder.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            await _builder.DisposeAsync();
        }

        public IClientStore Clients => _builder.Clients;
        public IRefreshTokenStore RefreshTokens => _builder.RefreshTokens;

        public TokenRequestValidator Validator => new TokenRequestValidator(
            clients: _host.Clients,
            refreshTokens: _host.RefreshTokens,
            profile: new TestProfileService(),
            clock: new Clock()
        );
    }
}
