using IdentityServer4.Models;
using System.Linq;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ApiScopeTests
    {
        [Fact]
        public void ConstructorWithNameAndDisplayName_ShouldSetBothProperties()
        {
            // Arrange
            var name = "testScope";
            var displayName = "Test Scope";

            // Act
            var scope = new ApiScope(name, displayName);

            // Assert
            Assert.Equal(name, scope.Name);
            Assert.Equal(displayName, scope.DisplayName);
            Assert.Empty(scope.UserClaims);
        }

        [Fact]
        public void ConstructorWithNameAndClaims_ShouldSetUserClaims()
        {
            // Arrange
            var name = "testScope";
            var claims = new[] { "claim1", "claim2" };

            // Act
            var scope = new ApiScope(name, claims);

            // Assert
            Assert.Equal(name, scope.Name);
            Assert.Equal(name, scope.DisplayName);
            Assert.Equal(claims, scope.UserClaims);
        }

        [Fact]
        public void ConstructorWithAllParameters_ShouldSetAllProperties()
        {
            // Arrange
            var name = "testScope";
            var displayName = "Test Scope";
            var claims = new[] { "claim1", "claim2" };

            // Act
            var scope = new ApiScope(name, displayName, claims);

            // Assert
            Assert.Equal(name, scope.Name);
            Assert.Equal(displayName, scope.DisplayName);
            Assert.Equal(claims, scope.UserClaims);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ConstructorWithInvalidName_ShouldThrowArgumentNullException(string name)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ApiScope(name));
        }
    }
}
