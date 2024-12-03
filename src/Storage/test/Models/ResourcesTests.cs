using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ResourcesTests
    {
        [Fact]
        public void EmptyResources_ShouldHaveEmptyCollections()
        {
            var resources = new Resources();

            resources.IdentityResources.Should().BeEmpty();
            resources.ApiResources.Should().BeEmpty();
            resources.ApiScopes.Should().BeEmpty();
            resources.OfflineAccess.Should().BeFalse();
        }

        [Fact]
        public void CopyConstructor_ShouldCopyAllProperties()
        {
            var identityResources = new[] { new IdentityResource("id1", new[] { "claim1" }) };
            var apiResources = new[] { new ApiResource("api1") };
            var apiScopes = new[] { new ApiScope("scope1") };

            var original = new Resources(identityResources, apiResources, apiScopes)
            {
                OfflineAccess = true
            };

            var copy = new Resources(original);

            copy.IdentityResources.Should().BeEquivalentTo(original.IdentityResources);
            copy.ApiResources.Should().BeEquivalentTo(original.ApiResources);
            copy.ApiScopes.Should().BeEquivalentTo(original.ApiScopes);
            copy.OfflineAccess.Should().Be(original.OfflineAccess);
        }

        [Fact]
        public void ParameterizedConstructor_WithNullCollections_ShouldCreateEmptyCollections()
        {
            var resources = new Resources(null, null, null);

            resources.IdentityResources.Should().BeEmpty();
            resources.ApiResources.Should().BeEmpty();
            resources.ApiScopes.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_WithValidCollections_ShouldInitializeProperties()
        {
            var identityResources = new[] { new IdentityResource("id1", new[] { "claim1" }) };
            var apiResources = new[] { new ApiResource("api1") };
            var apiScopes = new[] { new ApiScope("scope1") };

            var resources = new Resources(identityResources, apiResources, apiScopes);

            resources.IdentityResources.Should().BeEquivalentTo(identityResources);
            resources.ApiResources.Should().BeEquivalentTo(apiResources);
            resources.ApiScopes.Should().BeEquivalentTo(apiScopes);
        }
    }
}
