using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.UnitTests.Common
{
    public class SessionCookieManager
    {
        public bool EnsureSessionIdCookieWasCalled { get; set; }

        public async Task EnsureSessionIdCookieAsync()
        {
            EnsureSessionIdCookieWasCalled = true;
            await Task.CompletedTask;
        }
    }
}
