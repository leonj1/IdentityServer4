using System;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer4.UnitTests.Models
{
    public class PersistedGrantTests
    {
        [Fact]
        public void PersistedGrant_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var grant = new PersistedGrant
            {
                Key = "test-key",
                Type = "authorization_code",
                SubjectId = "123",
                SessionId = "session-123",
                ClientId = "client-123",
                Description = "Test Grant",
                CreationTime = new DateTime(2024, 1, 1),
                Expiration = new DateTime(2024, 12, 31),
                ConsumedTime = new DateTime(2024, 1, 2),
                Data = "{\"some\":\"data\"}"
            };

            // Assert
            Assert.Equal("test-key", grant.Key);
            Assert.Equal("authorization_code", grant.Type);
            Assert.Equal("123", grant.SubjectId);
            Assert.Equal("session-123", grant.SessionId);
            Assert.Equal("client-123", grant.ClientId);
            Assert.Equal("Test Grant", grant.Description);
            Assert.Equal(new DateTime(2024, 1, 1), grant.CreationTime);
            Assert.Equal(new DateTime(2024, 12, 31), grant.Expiration);
            Assert.Equal(new DateTime(2024, 1, 2), grant.ConsumedTime);
            Assert.Equal("{\"some\":\"data\"}", grant.Data);
        }

        [Fact]
        public void PersistedGrant_Expiration_Should_Allow_Null()
        {
            // Arrange
            var grant = new PersistedGrant
            {
                Key = "test-key",
                Expiration = null
            };

            // Assert
            Assert.Null(grant.Expiration);
        }

        [Fact]
        public void PersistedGrant_ConsumedTime_Should_Allow_Null()
        {
            // Arrange
            var grant = new PersistedGrant
            {
                Key = "test-key",
                ConsumedTime = null
            };

            // Assert
            Assert.Null(grant.ConsumedTime);
        }
    }
}
