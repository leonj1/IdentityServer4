using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemoryResourcesStoreTests
    {
        [Fact]
        public void should_throw_if_contains_duplicate_names()
        {
            List<IdentityResource> identityResources = new List<IdentityResource>
            {
                new IdentityResource { Name = "A" },
                new IdentityResource { Name = "A" },
                new IdentityResource { Name = "C" }
            };

            List<ApiResource> apiResources = new List<ApiResource>
            {
                new ApiResource { Name = "B" },
                new ApiResource { Name = "B" },
                new ApiResource { Name = "C" }
            };

            List<ApiScope> scopes = new List<ApiScope>
            {
                new ApiScope { Name = "B" },
                new ApiScope { Name = "C" },
                new ApiScope { Name = "C" },
            };

            Action act = () => new InMemoryResourcesStore(identityResources, null, null);
            act.Should().Throw<ArgumentException>();

            act = () => new InMemoryResourcesStore(null, apiResources, null);
            act.Should().Throw<ArgumentException>();
            
            act = () => new InMemoryResourcesStore(null, null, scopes);
            act.Should().Throw<ArgumentException>();
        }
    }
}
