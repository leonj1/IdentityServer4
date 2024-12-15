using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    internal class Scopes
    {
        public static IEnumerable<IdentityResource> GetIdentityScopes()
        {
            return new IdentityResource[]
            {
                IdentityResource.OpenId,
                IdentityResource.Email,
                IdentityResource.Address,
                IdentityResource.Roles
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "api",
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes = { "api1", "api2", "api3", "api4.with.roles" }
                },
                new ApiResource("other_api")
                {
                    Scopes = { "other_api" }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope
                {
                    Name = "api1"
                },
                new ApiScope
                {
                    Name = "api2"
                },
                new ApiScope
                {
                    Name = "api3"
                },
                new ApiScope
                {
                    Name = "api4.with.roles",
                    UserClaims = { "role" }
                },
                new ApiScope
                {
                    Name = "other_api",
                },
            };
        }
    }
}
