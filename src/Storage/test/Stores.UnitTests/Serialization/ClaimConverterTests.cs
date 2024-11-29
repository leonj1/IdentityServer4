using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Stores.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace IdentityServer.Stores.UnitTests.Serialization
{
    public class ClaimConverterTests
    {
        private readonly JsonSerializerSettings _settings;

        public ClaimConverterTests()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new ClaimConverter());
        }

        [Fact]
        public void Should_Serialize_And_Deserialize_Claim()
        {
            // Arrange
            var claim = new Claim("type", "value", "valueType");

            // Act
            var json = JsonConvert.SerializeObject(claim, _settings);
            var deserializedClaim = JsonConvert.DeserializeObject<Claim>(json, _settings);

            // Assert
            deserializedClaim.Should().NotBeNull();
            deserializedClaim.Type.Should().Be(claim.Type);
            deserializedClaim.Value.Should().Be(claim.Value);
            deserializedClaim.ValueType.Should().Be(claim.ValueType);
        }

        [Fact]
        public void CanConvert_Should_Return_True_For_Claim_Type()
        {
            // Arrange
            var converter = new ClaimConverter();

            // Act
            var result = converter.CanConvert(typeof(Claim));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanConvert_Should_Return_False_For_Other_Types()
        {
            // Arrange
            var converter = new ClaimConverter();

            // Act
            var result = converter.CanConvert(typeof(string));

            // Assert
            result.Should().BeFalse();
        }
    }
}
