using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class IdentityResource
    {
        public static IdentityResources.OpenId OpenId { get; } = new IdentityResources.OpenId();
        public static IdentityResources.Email Email { get; } = new IdentityResources.Email();
        public static IdentityResources.Address Address { get; } = new IdentityResources.Address();
        public static IdentityResource Roles { get; } = new IdentityResource("roles", new[] { "role" });
    }
}
