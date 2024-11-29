using System;
using System.Security.Claims;
using System.Linq;
using Xunit;
using Newtonsoft.Json;
using IdentityModel;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class ClaimsPrincipalConverterTests
    {
        private readonly JsonSerializerSettings _settings;
        
        public ClaimsPrincipalConverterTests()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new ClaimsPrincipalConverter());
        }

        [Fact]
        public void Should_Serialize_And_Deserialize_ClaimsPrincipal()
        {
            // Arrange
            var claims = new[]
            {
                new Claim("type1", "value1", "valueType1"),
                new Claim("type2", "value2", "valueType2")
            };
            
            var identity = new ClaimsIdentity(claims, "test-auth", JwtClaimTypes.Name, JwtClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var json = JsonConvert.SerializeObject(principal, _settings);
            var deserializedPrincipal = JsonConvert.DeserializeObject<ClaimsPrincipal>(json, _settings);

            // Assert
            Assert.NotNull(deserializedPrincipal);
            Assert.NotNull(deserializedPrincipal.Identity);
            Assert.Equal("test-auth", deserializedPrincipal.Identity.AuthenticationType);
            
            var deserializedClaims = deserializedPrincipal.Claims.ToList();
            Assert.Equal(2, deserializedClaims.Count);
            
            Assert.Equal("type1", deserializedClaims[0].Type);
            Assert.Equal("value1", deserializedClaims[0].Value);
            Assert.Equal("valueType1", deserializedClaims[0].ValueType);
            
            Assert.Equal("type2", deserializedClaims[1].Type);
            Assert.Equal("value2", deserializedClaims[1].Value);
            Assert.Equal("valueType2", deserializedClaims[1].ValueType);
        }

        [Fact]
        public void Should_Handle_Null_ClaimsPrincipal()
        {
            // Act
            var json = JsonConvert.SerializeObject(null, _settings);
            var deserializedPrincipal = JsonConvert.DeserializeObject<ClaimsPrincipal>(json, _settings);

            // Assert
            Assert.Null(deserializedPrincipal);
        }
    }
}
