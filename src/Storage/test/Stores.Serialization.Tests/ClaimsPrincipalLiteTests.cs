using System;
using System.Collections.Generic;
using Xunit;
using IdentityServer4.Stores.Serialization;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class ClaimsPrincipalLiteTests
    {
        [Fact]
        public void Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var claimsPrincipalLite = new ClaimsPrincipalLite();
            var authType = "TestAuth";
            var claims = new[]
            {
                new ClaimLite { Type = "claim1", Value = "value1" },
                new ClaimLite { Type = "claim2", Value = "value2" }
            };

            // Act
            claimsPrincipalLite.AuthenticationType = authType;
            claimsPrincipalLite.Claims = claims;

            // Assert
            Assert.Equal(authType, claimsPrincipalLite.AuthenticationType);
            Assert.Equal(claims, claimsPrincipalLite.Claims);
            Assert.Equal(2, claimsPrincipalLite.Claims.Length);
            Assert.Equal("claim1", claimsPrincipalLite.Claims[0].Type);
            Assert.Equal("value1", claimsPrincipalLite.Claims[0].Value);
            Assert.Equal("claim2", claimsPrincipalLite.Claims[1].Type);
            Assert.Equal("value2", claimsPrincipalLite.Claims[1].Value);
        }

        [Fact]
        public void Properties_Should_Allow_Null_Values()
        {
            // Arrange
            var claimsPrincipalLite = new ClaimsPrincipalLite();

            // Act & Assert
            Assert.Null(claimsPrincipalLite.AuthenticationType);
            Assert.Null(claimsPrincipalLite.Claims);
        }
    }
}
