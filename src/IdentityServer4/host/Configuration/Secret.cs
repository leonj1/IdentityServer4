using IdentityServer4.Models;

namespace IdentityServerHost.Configuration
{
    public static class Secret
    {
        public static IEnumerable<Secret> Get()
        {
            return new List<Secret>
            {
                new Secret("secret".Sha256())
            };
        }
    }
}
