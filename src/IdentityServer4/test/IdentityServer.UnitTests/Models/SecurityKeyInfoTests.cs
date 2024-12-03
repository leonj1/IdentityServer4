using System;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class SecurityKeyInfoTests
    {
        [Fact]
        public void SecurityKeyInfo_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var key = new SymmetricSecurityKey(new byte[] { 1, 2, 3, 4, 5 });
            var algorithm = "HS256";

            // Act
            var securityKeyInfo = new IdentityServer4.Models.SecurityKeyInfo
            {
                Key = key,
                SigningAlgorithm = algorithm
            };

            // Assert
            securityKeyInfo.Key.Should().Be(key);
            securityKeyInfo.SigningAlgorithm.Should().Be(algorithm);
        }

        [Fact]
        public void SecurityKeyInfo_Should_Allow_Null_Values()
        {
            // Arrange & Act
            var securityKeyInfo = new IdentityServer4.Models.SecurityKeyInfo();

            // Assert
            securityKeyInfo.Key.Should().BeNull();
            securityKeyInfo.SigningAlgorithm.Should().BeNull();
        }
    }
}
