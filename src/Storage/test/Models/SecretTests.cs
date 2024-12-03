using System;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class SecretTests
    {
        [Fact]
        public void DefaultConstructor_SetsCorrectType()
        {
            var secret = new Secret();
            Assert.Equal(IdentityServerConstants.SecretTypes.SharedSecret, secret.Type);
        }

        [Fact]
        public void ConstructorWithValue_SetsValueAndType()
        {
            var value = "secret";
            var secret = new Secret(value);
            
            Assert.Equal(value, secret.Value);
            Assert.Equal(IdentityServerConstants.SecretTypes.SharedSecret, secret.Type);
            Assert.Null(secret.Expiration);
        }

        [Fact]
        public void ConstructorWithValueAndExpiration_SetsAllProperties()
        {
            var value = "secret";
            var expiration = DateTime.UtcNow.AddDays(1);
            var secret = new Secret(value, expiration);
            
            Assert.Equal(value, secret.Value);
            Assert.Equal(expiration, secret.Expiration);
            Assert.Equal(IdentityServerConstants.SecretTypes.SharedSecret, secret.Type);
        }

        [Fact]
        public void ConstructorWithDescription_SetsAllProperties()
        {
            var value = "secret";
            var description = "test secret";
            var expiration = DateTime.UtcNow.AddDays(1);
            var secret = new Secret(value, description, expiration);
            
            Assert.Equal(value, secret.Value);
            Assert.Equal(description, secret.Description);
            Assert.Equal(expiration, secret.Expiration);
            Assert.Equal(IdentityServerConstants.SecretTypes.SharedSecret, secret.Type);
        }

        [Fact]
        public void Equals_WithSameValueAndType_ReturnsTrue()
        {
            var secret1 = new Secret("secret");
            var secret2 = new Secret("secret");
            
            Assert.True(secret1.Equals(secret2));
        }

        [Fact]
        public void Equals_WithDifferentValue_ReturnsFalse()
        {
            var secret1 = new Secret("secret1");
            var secret2 = new Secret("secret2");
            
            Assert.False(secret1.Equals(secret2));
        }

        [Fact]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            var secret1 = new Secret("secret") { Type = "type1" };
            var secret2 = new Secret("secret") { Type = "type2" };
            
            Assert.False(secret1.Equals(secret2));
        }

        [Fact]
        public void Equals_WithNull_ReturnsFalse()
        {
            var secret = new Secret("secret");
            Assert.False(secret.Equals(null));
        }

        [Fact]
        public void GetHashCode_SameValueAndType_ReturnsSameHash()
        {
            var secret1 = new Secret("secret");
            var secret2 = new Secret("secret");
            
            Assert.Equal(secret1.GetHashCode(), secret2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentValues_ReturnsDifferentHash()
        {
            var secret1 = new Secret("secret1");
            var secret2 = new Secret("secret2");
            
            Assert.NotEqual(secret1.GetHashCode(), secret2.GetHashCode());
        }
    }
}
