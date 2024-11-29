using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class DeviceCodeTests
    {
        [Fact]
        public void DeviceCode_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var deviceCode = new DeviceCode
            {
                CreationTime = new DateTime(2024, 1, 1),
                Lifetime = 300,
                ClientId = "test_client",
                Description = "test device",
                IsOpenId = true,
                IsAuthorized = true,
                RequestedScopes = new[] { "scope1", "scope2" },
                AuthorizedScopes = new[] { "scope1" },
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "123") })),
                SessionId = "session_123"
            };

            // Assert
            Assert.Equal(new DateTime(2024, 1, 1), deviceCode.CreationTime);
            Assert.Equal(300, deviceCode.Lifetime);
            Assert.Equal("test_client", deviceCode.ClientId);
            Assert.Equal("test device", deviceCode.Description);
            Assert.True(deviceCode.IsOpenId);
            Assert.True(deviceCode.IsAuthorized);
            Assert.Equal(new[] { "scope1", "scope2" }, deviceCode.RequestedScopes);
            Assert.Equal(new[] { "scope1" }, deviceCode.AuthorizedScopes);
            Assert.Equal("123", deviceCode.Subject.FindFirst("sub").Value);
            Assert.Equal("session_123", deviceCode.SessionId);
        }

        [Fact]
        public void DeviceCode_Should_Handle_Null_Properties()
        {
            // Arrange
            var deviceCode = new DeviceCode();

            // Assert
            Assert.Equal(default(DateTime), deviceCode.CreationTime);
            Assert.Equal(0, deviceCode.Lifetime);
            Assert.Null(deviceCode.ClientId);
            Assert.Null(deviceCode.Description);
            Assert.False(deviceCode.IsOpenId);
            Assert.False(deviceCode.IsAuthorized);
            Assert.Null(deviceCode.RequestedScopes);
            Assert.Null(deviceCode.AuthorizedScopes);
            Assert.Null(deviceCode.Subject);
            Assert.Null(deviceCode.SessionId);
        }
    }
}
