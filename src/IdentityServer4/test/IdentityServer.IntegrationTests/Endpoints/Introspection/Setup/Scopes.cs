using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    internal class Scopes
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource
                {
                    Name = "api1",
                    ApiSecrets = new List<Secret>
                    {
                        Secret.Sha256("secret")
                    },
                    Scopes = { "api1" }
                },
                new ApiResource
                {
                    Name = "api2",
                    ApiSecrets = new List<Secret>
                    {
                        Secret.Sha256("secret")
                    },
                    Scopes = { "api2" }
                },
                 new ApiResource
                {
                    Name = "api3",
                    ApiSecrets = new List<Secret>
                    {
                        Secret.Sha256("secret")
                    },
                    Scopes = { "api3-a", "api3-b" }
                }
            };
        }

        public static IEnumerable<ApiScope> GetScopes()
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
                    Name = "api3-a"
                },
                new ApiScope
                {
                    Name = "api3-b"
                }
            };
        }
    }
}
