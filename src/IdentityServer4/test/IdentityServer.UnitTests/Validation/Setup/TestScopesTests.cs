using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestScopesTests
    {
        [Fact]
        public void GetIdentity_Should_Return_Expected_Identity_Resources()
        {
            // Act
            var identityResources = TestScopes.GetIdentity().ToList();

            // Assert
            identityResources.Should().HaveCount(2);
            identityResources[0].Name.Should().Be("openid");
            identityResources[1].Name.Should().Be("profile");
        }

        [Fact]
        public void GetApis_Should_Return_Expected_Api_Resources()
        {
            // Act
            var apiResources = TestScopes.GetApis().ToList();

            // Assert
            apiResources.Should().HaveCount(1);
            apiResources[0].Name.Should().Be("api");
            apiResources[0].Scopes.Should().Contain(new[] { "resource", "resource2" });
        }

        [Fact]
        public void GetScopes_Should_Return_Expected_Api_Scopes()
        {
            // Act
            var apiScopes = TestScopes.GetScopes().ToList();

            // Assert
            apiScopes.Should().HaveCount(2);
            
            apiScopes[0].Name.Should().Be("resource");
            apiScopes[0].Description.Should().Be("resource scope");
            
            apiScopes[1].Name.Should().Be("resource2");
            apiScopes[1].Description.Should().Be("resource scope");
        }
    }
}
