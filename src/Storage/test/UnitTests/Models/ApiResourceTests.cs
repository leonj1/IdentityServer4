using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace UnitTests.Models
{
    public class ApiResourceTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeCollections()
        {
            // Act
            var resource = new ApiResource();

            // Assert
            resource.ApiSecrets.Should().NotBeNull().And.BeEmpty();
            resource.Scopes.Should().NotBeNull().And.BeEmpty();
            resource.AllowedAccessTokenSigningAlgorithms.Should().NotBeNull().And.BeEmpty();
            resource.UserClaims.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void NameOnlyConstructor_ShouldSetNameAndDisplayName()
        {
            // Act
            var resource = new ApiResource("TestApi");

            // Assert
            resource.Name.Should().Be("TestApi");
            resource.DisplayName.Should().Be("TestApi");
        }

        [Fact]
        public void NameAndDisplayNameConstructor_ShouldSetBothProperties()
        {
            // Act
            var resource = new ApiResource("TestApi", "Test API Display");

            // Assert
            resource.Name.Should().Be("TestApi");
            resource.DisplayName.Should().Be("Test API Display");
        }

        [Fact]
        public void NameAndClaimsConstructor_ShouldSetNameAndClaims()
        {
            // Arrange
            var claims = new[] { "claim1", "claim2" };

            // Act
            var resource = new ApiResource("TestApi", claims);

            // Assert
            resource.Name.Should().Be("TestApi");
            resource.DisplayName.Should().Be("TestApi");
            resource.UserClaims.Should().BeEquivalentTo(claims);
        }

        [Fact]
        public void FullConstructor_ShouldSetAllProperties()
        {
            // Arrange
            var claims = new[] { "claim1", "claim2" };

            // Act
            var resource = new ApiResource("TestApi", "Test API Display", claims);

            // Assert
            resource.Name.Should().Be("TestApi");
            resource.DisplayName.Should().Be("Test API Display");
            resource.UserClaims.Should().BeEquivalentTo(claims);
        }

        [Fact]
        public void ConstructorWithNullName_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ApiResource(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddingScopes_ShouldWorkCorrectly()
        {
            // Arrange
            var resource = new ApiResource("TestApi");
            var scopes = new[] { "scope1", "scope2" };

            // Act
            foreach (var scope in scopes)
            {
                resource.Scopes.Add(scope);
            }

            // Assert
            resource.Scopes.Should().BeEquivalentTo(scopes);
        }

        [Fact]
        public void AddingSecrets_ShouldWorkCorrectly()
        {
            // Arrange
            var resource = new ApiResource("TestApi");
            var secret = new Secret("secret");

            // Act
            resource.ApiSecrets.Add(secret);

            // Assert
            resource.ApiSecrets.Should().ContainSingle()
                .Which.Value.Should().Be(secret.Value);
        }
    }
}
