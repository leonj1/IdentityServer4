using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Entities
{
    public class ApiResourceSecretTests
    {
        [Fact]
        public void ApiResourceSecret_Properties_Can_Be_Set_And_Retrieved()
        {
            // Arrange
            var apiResourceSecret = new ApiResourceSecret
            {
                ApiResourceId = 123,
                ApiResource = new ApiResource { Name = "test-api" }
            };

            // Act & Assert
            Assert.Equal(123, apiResourceSecret.ApiResourceId);
            Assert.NotNull(apiResourceSecret.ApiResource);
            Assert.Equal("test-api", apiResourceSecret.ApiResource.Name);
        }

        [Fact]
        public void ApiResourceSecret_Inherits_From_Secret()
        {
            // Arrange & Act
            var apiResourceSecret = new ApiResourceSecret();

            // Assert
            Assert.True(apiResourceSecret is Secret);
        }
    }
}
