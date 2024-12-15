using System;
using IdentityModel;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class TokenTests_DefaultConstructor
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeCollections()
        {
            // Arrange & Act
            var token = new Token();

            // Assert
            Assert.NotNull(token.AllowedSigningAlgorithms);
            Assert.NotNull(token.Audiences);
            Assert.NotNull(token.Claims);
            Assert.Empty(token.AllowedSigningAlgorithms);
            Assert.Empty(token.Audiences);
            Assert.Empty(token.Claims);
        }
    }
}
