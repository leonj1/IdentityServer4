using System;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Entities
{
    public class ApiResourceTests
    {
        [Fact]
        public void ApiResource_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var apiResource = new ApiResource();

            // Assert
            apiResource.Enabled.Should().BeTrue();
            apiResource.ShowInDiscoveryDocument.Should().BeTrue();
            apiResource.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            apiResource.NonEditable.Should().BeFalse();
        }

        [Fact]
        public void ApiResource_Collections_AreInitialized()
        {
            // Act
            var apiResource = new ApiResource();

            // Assert
            apiResource.Secrets.Should().NotBeNull();
            apiResource.Scopes.Should().NotBeNull();
            apiResource.UserClaims.Should().NotBeNull();
            apiResource.Properties.Should().NotBeNull();
        }

        [Fact]
        public void ApiResource_Properties_CanBeSet()
        {
            // Arrange
            var apiResource = new ApiResource
            {
                Name = "test-api",
                DisplayName = "Test API",
                Description = "Test API Description",
                AllowedAccessTokenSigningAlgorithms = "RS256",
                Enabled = false,
                ShowInDiscoveryDocument = false,
                NonEditable = true
            };

            // Assert
            apiResource.Name.Should().Be("test-api");
            apiResource.DisplayName.Should().Be("Test API");
            apiResource.Description.Should().Be("Test API Description");
            apiResource.AllowedAccessTokenSigningAlgorithms.Should().Be("RS256");
            apiResource.Enabled.Should().BeFalse();
            apiResource.ShowInDiscoveryDocument.Should().BeFalse();
            apiResource.NonEditable.Should().BeTrue();
        }

        [Fact]
        public void ApiResource_Collections_CanBeModified()
        {
            // Arrange
            var apiResource = new ApiResource();
            var secret = new ApiResourceSecret { Value = "secret" };
            var scope = new ApiResourceScope { Scope = "scope1" };
            var claim = new ApiResourceClaim { Type = "claim1" };
            var property = new ApiResourceProperty { Key = "key1", Value = "value1" };

            // Act
            apiResource.Secrets = new List<ApiResourceSecret> { secret };
            apiResource.Scopes = new List<ApiResourceScope> { scope };
            apiResource.UserClaims = new List<ApiResourceClaim> { claim };
            apiResource.Properties = new List<ApiResourceProperty> { property };

            // Assert
            apiResource.Secrets.Should().ContainSingle().Which.Value.Should().Be("secret");
            apiResource.Scopes.Should().ContainSingle().Which.Scope.Should().Be("scope1");
            apiResource.UserClaims.Should().ContainSingle().Which.Type.Should().Be("claim1");
            apiResource.Properties.Should().ContainSingle().Which.Key.Should().Be("key1");
        }

        [Fact]
        public void ApiResource_UpdatedAndLastAccessed_CanBeSet()
        {
            // Arrange
            var apiResource = new ApiResource();
            var updateTime = DateTime.UtcNow;
            var accessTime = DateTime.UtcNow.AddMinutes(5);

            // Act
            apiResource.Updated = updateTime;
            apiResource.LastAccessed = accessTime;

            // Assert
            apiResource.Updated.Should().Be(updateTime);
            apiResource.LastAccessed.Should().Be(accessTime);
        }
    }
}
