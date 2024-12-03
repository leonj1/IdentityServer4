using System;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class PersistentGrantSerializerTests
    {
        private readonly PersistentGrantSerializer _subject = new PersistentGrantSerializer();

        [Fact]
        public void Serialize_and_Deserialize_DeviceCode_should_work()
        {
            // Arrange
            var deviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = new DateTime(2024, 1, 1),
                Data = "some data",
                Lifetime = 3600,
                SubjectId = "123"
            };

            // Act
            var json = _subject.Serialize(deviceCode);
            var result = _subject.Deserialize<DeviceCode>(json);

            // Assert
            result.Should().BeEquivalentTo(deviceCode);
        }

        [Fact]
        public void Serialize_and_Deserialize_with_ClaimsPrincipal_should_work()
        {
            // Arrange
            var claims = new Claim[]
            {
                new Claim("sub", "123"),
                new Claim("name", "Bob"),
                new Claim("email", "bob@example.com")
            };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            // Act
            var json = _subject.Serialize(principal);
            var result = _subject.Deserialize<ClaimsPrincipal>(json);

            // Assert
            result.Claims.Should().BeEquivalentTo(principal.Claims);
        }

        [Fact]
        public void Deserialize_invalid_json_should_return_null()
        {
            // Act
            var result = _subject.Deserialize<DeviceCode>("invalid json");

            // Assert
            result.Should().BeNull();
        }
    }
}
