using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ResourceValidationResultTests
    {
        [Fact]
        public void Empty_Constructor_Should_Initialize_Properties()
        {
            var result = new ResourceValidationResult();

            result.Resources.Should().NotBeNull();
            result.ParsedScopes.Should().NotBeNull();
            result.InvalidScopes.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public void Resources_Constructor_Should_Initialize_Correctly()
        {
            var resources = new Resources(
                new IdentityResource[] { new IdentityResources.OpenId() },
                new ApiResource[] { new ApiResource("api1") },
                new ApiScope[] { new ApiScope("scope1") }
            );

            var result = new ResourceValidationResult(resources);

            result.Resources.Should().Be(resources);
            result.ParsedScopes.Should().NotBeEmpty();
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void Filter_Should_Return_Filtered_Resources()
        {
            var resources = new Resources(
                new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile() },
                new ApiResource[] { new ApiResource("api1", scopes: new[] { "scope1" }) },
                new ApiScope[] { new ApiScope("scope1") }
            );

            var result = new ResourceValidationResult(resources);
            var filtered = result.Filter(new[] { "openid" });

            filtered.Resources.IdentityResources.Should().ContainSingle();
            filtered.Resources.IdentityResources.First().Name.Should().Be("openid");
            filtered.Resources.ApiResources.Should().BeEmpty();
            filtered.Resources.ApiScopes.Should().BeEmpty();
        }

        [Fact]
        public void Filter_Should_Handle_OfflineAccess()
        {
            var resources = new Resources(
                new IdentityResource[] { new IdentityResources.OpenId() },
                new ApiResource[] { new ApiResource("api1") },
                new ApiScope[] { new ApiScope("scope1") }
            );

            var result = new ResourceValidationResult(resources);
            var filtered = result.Filter(new[] { "offline_access" });

            filtered.Resources.OfflineAccess.Should().BeTrue();
            filtered.Resources.IdentityResources.Should().BeEmpty();
            filtered.Resources.ApiResources.Should().BeEmpty();
            filtered.Resources.ApiScopes.Should().BeEmpty();
        }

        [Fact]
        public void Filter_Should_Handle_Null_Input()
        {
            var resources = new Resources(
                new IdentityResource[] { new IdentityResources.OpenId() },
                new ApiResource[] { new ApiResource("api1") },
                new ApiScope[] { new ApiScope("scope1") }
            );

            var result = new ResourceValidationResult(resources);
            var filtered = result.Filter(null);

            filtered.Resources.IdentityResources.Should().BeEmpty();
            filtered.Resources.ApiResources.Should().BeEmpty();
            filtered.Resources.ApiScopes.Should().BeEmpty();
            filtered.ParsedScopes.Should().BeEmpty();
        }
    }
}
