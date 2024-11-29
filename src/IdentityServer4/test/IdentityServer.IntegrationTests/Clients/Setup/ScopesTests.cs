using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class ScopesTests
    {
        [Fact]
        public void GetIdentityScopes_ShouldReturnCorrectScopes()
        {
            // Act
            var scopes = Scopes.GetIdentityScopes().ToList();

            // Assert
            scopes.Should().HaveCount(4);
            scopes.Should().Contain(x => x.Name == "openid");
            scopes.Should().Contain(x => x.Name == "email");
            scopes.Should().Contain(x => x.Name == "address");
            scopes.Should().Contain(x => x.Name == "roles" && x.UserClaims.Contains("role"));
        }

        [Fact]
        public void GetApiResources_ShouldReturnCorrectResources()
        {
            // Act
            var resources = Scopes.GetApiResources().ToList();

            // Assert
            resources.Should().HaveCount(2);
            
            var apiResource = resources.First(x => x.Name == "api");
            apiResource.ApiSecrets.Should().HaveCount(1);
            apiResource.Scopes.Should().Contain(new[] { "api1", "api2", "api3", "api4.with.roles" });

            var otherApi = resources.First(x => x.Name == "other_api");
            otherApi.Scopes.Should().Contain("other_api");
        }

        [Fact]
        public void GetApiScopes_ShouldReturnCorrectScopes()
        {
            // Act
            var scopes = Scopes.GetApiScopes().ToList();

            // Assert
            scopes.Should().HaveCount(5);
            scopes.Should().Contain(x => x.Name == "api1");
            scopes.Should().Contain(x => x.Name == "api2");
            scopes.Should().Contain(x => x.Name == "api3");
            
            var roleScope = scopes.First(x => x.Name == "api4.with.roles");
            roleScope.UserClaims.Should().Contain("role");

            scopes.Should().Contain(x => x.Name == "other_api");
        }
    }
}
