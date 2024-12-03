using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    public class ScopesTests
    {
        [Fact]
        public void GetApis_ShouldReturnCorrectApiResources()
        {
            // Act
            var apis = Scopes.GetApis().ToList();

            // Assert
            apis.Should().HaveCount(3);
            
            // Verify api1
            var api1 = apis[0];
            api1.Name.Should().Be("api1");
            api1.ApiSecrets.Should().ContainSingle();
            api1.Scopes.Should().ContainSingle().Which.Should().Be("api1");

            // Verify api2
            var api2 = apis[1];
            api2.Name.Should().Be("api2");
            api2.ApiSecrets.Should().ContainSingle();
            api2.Scopes.Should().ContainSingle().Which.Should().Be("api2");

            // Verify api3
            var api3 = apis[2];
            api3.Name.Should().Be("api3");
            api3.ApiSecrets.Should().ContainSingle();
            api3.Scopes.Should().HaveCount(2);
            api3.Scopes.Should().Contain("api3-a");
            api3.Scopes.Should().Contain("api3-b");
        }

        [Fact]
        public void GetScopes_ShouldReturnCorrectApiScopes()
        {
            // Act
            var scopes = Scopes.GetScopes().ToList();

            // Assert
            scopes.Should().HaveCount(4);

            scopes[0].Name.Should().Be("api1");
            scopes[1].Name.Should().Be("api2");
            scopes[2].Name.Should().Be("api3-a");
            scopes[3].Name.Should().Be("api3-b");
        }
    }
}
