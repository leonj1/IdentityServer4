using Xunit;
using IdentityServer4.Models;
using System.Linq;
using IdentityModel;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServerHost.Configuration.Tests
{
    public class ResourcesTests
    {
        [Fact]
        public void IdentityResources_ShouldContainExpectedResources()
        {
            var resources = Resources.IdentityResources.ToList();
            
            Assert.Equal(4, resources.Count);
            Assert.Contains(resources, r => r.Name == "openid");
            Assert.Contains(resources, r => r.Name == "profile");
            Assert.Contains(resources, r => r.Name == "email");
            Assert.Contains(resources, r => r.Name == "custom.profile");
        }

        [Fact]
        public void CustomProfile_ShouldHaveCorrectClaims()
        {
            var customProfile = Resources.IdentityResources
                .First(r => r.Name == "custom.profile");

            Assert.Contains(JwtClaimTypes.Name, customProfile.UserClaims);
            Assert.Contains(JwtClaimTypes.Email, customProfile.UserClaims);
            Assert.Contains("location", customProfile.UserClaims);
        }

        [Fact]
        public void ApiScopes_ShouldContainExpectedScopes()
        {
            var scopes = Resources.ApiScopes.ToList();

            Assert.Equal(6, scopes.Count);
            Assert.Contains(scopes, s => s.Name == LocalApi.ScopeName);
            Assert.Contains(scopes, s => s.Name == "resource1.scope1");
            Assert.Contains(scopes, s => s.Name == "resource2.scope1");
            Assert.Contains(scopes, s => s.Name == "scope3");
            Assert.Contains(scopes, s => s.Name == "shared.scope");
            Assert.Contains(scopes, s => s.Name == "transaction");
        }

        [Fact]
        public void ApiResources_ShouldContainExpectedResources()
        {
            var resources = Resources.ApiResources.ToList();

            Assert.Equal(2, resources.Count);
            Assert.Contains(resources, r => r.Name == "resource1");
            Assert.Contains(resources, r => r.Name == "resource2");
        }

        [Fact]
        public void Resource2_ShouldHaveCorrectClaims()
        {
            var resource2 = Resources.ApiResources
                .First(r => r.Name == "resource2");

            Assert.Contains(JwtClaimTypes.Name, resource2.UserClaims);
            Assert.Contains(JwtClaimTypes.Email, resource2.UserClaims);
        }

        [Fact]
        public void ApiResources_ShouldHaveCorrectScopes()
        {
            var resource1 = Resources.ApiResources
                .First(r => r.Name == "resource1");
            var resource2 = Resources.ApiResources
                .First(r => r.Name == "resource2");

            Assert.Contains("resource1.scope1", resource1.Scopes);
            Assert.Contains("shared.scope", resource1.Scopes);
            Assert.Contains("resource2.scope1", resource2.Scopes);
            Assert.Contains("shared.scope", resource2.Scopes);
        }
    }
}
