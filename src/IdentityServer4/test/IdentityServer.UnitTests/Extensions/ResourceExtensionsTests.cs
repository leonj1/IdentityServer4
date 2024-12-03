using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ResourceExtensionsTests
    {
        [Fact]
        public void ToScopeNames_Should_Return_Correct_Scope_Names()
        {
            // Arrange
            var resources = new Resources(
                new IdentityResource[] 
                { 
                    new IdentityResource("openid", "OpenID")
                },
                new ApiResource[] { },
                new ApiScope[] 
                { 
                    new ApiScope("api1", "API 1")
                })
            {
                OfflineAccess = true
            };

            // Act
            var scopeNames = resources.ToScopeNames();

            // Assert
            scopeNames.Should().BeEquivalentTo(new[] 
            { 
                "openid", 
                "api1",
                "offline_access"
            });
        }

        [Fact]
        public void FindIdentityResourcesByScope_Should_Return_Correct_Resource()
        {
            // Arrange
            var resources = new Resources(
                new IdentityResource[] 
                { 
                    new IdentityResource("openid", "OpenID"),
                    new IdentityResource("profile", "Profile")
                },
                new ApiResource[] { },
                new ApiScope[] { });

            // Act
            var resource = resources.FindIdentityResourcesByScope("profile");

            // Assert
            resource.Should().NotBeNull();
            resource.Name.Should().Be("profile");
        }

        [Fact]
        public void FindApiScope_Should_Return_Correct_Scope()
        {
            // Arrange
            var resources = new Resources(
                new IdentityResource[] { },
                new ApiResource[] { },
                new ApiScope[] 
                { 
                    new ApiScope("api1", "API 1"),
                    new ApiScope("api2", "API 2")
                });

            // Act
            var scope = resources.FindApiScope("api1");

            // Assert
            scope.Should().NotBeNull();
            scope.Name.Should().Be("api1");
        }

        [Fact]
        public void FilterEnabled_Should_Only_Return_Enabled_Resources()
        {
            // Arrange
            var resources = new Resources(
                new IdentityResource[] 
                { 
                    new IdentityResource("openid", "OpenID") { Enabled = true },
                    new IdentityResource("profile", "Profile") { Enabled = false }
                },
                new ApiResource[] 
                {
                    new ApiResource("api1", "API 1") { Enabled = true },
                    new ApiResource("api2", "API 2") { Enabled = false }
                },
                new ApiScope[] 
                { 
                    new ApiScope("scope1", "Scope 1") { Enabled = true },
                    new ApiScope("scope2", "Scope 2") { Enabled = false }
                });

            // Act
            var filteredResources = resources.FilterEnabled();

            // Assert
            filteredResources.IdentityResources.Should().HaveCount(1);
            filteredResources.IdentityResources.First().Name.Should().Be("openid");
            
            filteredResources.ApiResources.Should().HaveCount(1);
            filteredResources.ApiResources.First().Name.Should().Be("api1");
            
            filteredResources.ApiScopes.Should().HaveCount(1);
            filteredResources.ApiScopes.First().Name.Should().Be("scope1");
        }

        [Fact]
        public void FindMatchingSigningAlgorithms_Should_Return_Common_Algorithms()
        {
            // Arrange
            var apiResources = new[]
            {
                new ApiResource("api1")
                {
                    AllowedAccessTokenSigningAlgorithms = new[] { "RS256", "PS256" }
                },
                new ApiResource("api2")
                {
                    AllowedAccessTokenSigningAlgorithms = new[] { "RS256", "ES256" }
                }
            };

            // Act
            var algorithms = apiResources.FindMatchingSigningAlgorithms();

            // Assert
            algorithms.Should().ContainSingle();
            algorithms.Should().Contain("RS256");
        }

        [Fact]
        public void GetRequiredScopeValues_Should_Return_Required_Scopes()
        {
            // Arrange
            var resources = new Resources(
                new IdentityResource[] 
                { 
                    new IdentityResource("openid", "OpenID") { Required = true }
                },
                new ApiResource[] { },
                new ApiScope[] 
                { 
                    new ApiScope("api1", "API 1") { Required = true }
                });

            var parsedScopes = new ParsedScopeValue[] 
            {
                new ParsedScopeValue("openid"),
                new ParsedScopeValue("api1"),
                new ParsedScopeValue("optional")
            };

            var result = new ResourceValidationResult(resources, parsedScopes);

            // Act
            var requiredScopes = result.GetRequiredScopeValues();

            // Assert
            requiredScopes.Should().BeEquivalentTo(new[] { "openid", "api1" });
        }
    }
}
