using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer4.UnitTests.Models
{
    public class ConsentTests
    {
        [Fact]
        public void Constructor_CreatesValidInstance()
        {
            var consent = new Consent();
            Assert.NotNull(consent);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = "subject123",
                ClientId = "client123",
                Scopes = new[] { "scope1", "scope2" },
                CreationTime = new DateTime(2024, 1, 1),
                Expiration = new DateTime(2024, 12, 31)
            };

            // Assert
            Assert.Equal("subject123", consent.SubjectId);
            Assert.Equal("client123", consent.ClientId);
            Assert.Equal(2, consent.Scopes.Count());
            Assert.Contains("scope1", consent.Scopes);
            Assert.Contains("scope2", consent.Scopes);
            Assert.Equal(new DateTime(2024, 1, 1), consent.CreationTime);
            Assert.Equal(new DateTime(2024, 12, 31), consent.Expiration);
        }

        [Fact]
        public void Expiration_CanBeNull()
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = "subject123",
                ClientId = "client123",
                Scopes = new[] { "scope1" },
                CreationTime = DateTime.UtcNow,
                Expiration = null
            };

            // Assert
            Assert.Null(consent.Expiration);
        }

        [Fact]
        public void Scopes_CanBeEmpty()
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = "subject123",
                ClientId = "client123",
                Scopes = new List<string>(),
                CreationTime = DateTime.UtcNow
            };

            // Assert
            Assert.Empty(consent.Scopes);
        }

        [Fact]
        public void Scopes_CanContainDuplicates()
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = "subject123",
                ClientId = "client123",
                Scopes = new[] { "scope1", "scope1", "scope2" }
            };

            // Assert
            Assert.Equal(3, consent.Scopes.Count());
            Assert.Equal(2, consent.Scopes.Count(s => s == "scope1"));
        }

        [Fact]
        public void CreationTime_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;
            var consent = new Consent();
            var after = DateTime.UtcNow;

            // Assert
            Assert.InRange(consent.CreationTime, before, after);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SubjectId_CannotBeNullOrWhitespace(string subjectId)
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = subjectId,
                ClientId = "client123"
            };

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(consent.SubjectId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ClientId_CannotBeNullOrWhitespace(string clientId)
        {
            // Arrange
            var consent = new Consent
            {
                SubjectId = "subject123",
                ClientId = clientId
            };

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(consent.ClientId));
        }
    }
}
