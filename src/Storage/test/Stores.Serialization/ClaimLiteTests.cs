using IdentityServer4.Stores.Serialization;
using Xunit;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class ClaimLiteTests
    {
        [Fact]
        public void ClaimLite_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var claimLite = new ClaimLite
            {
                Type = "claim_type",
                Value = "claim_value",
                ValueType = "claim_value_type"
            };

            // Assert
            Assert.Equal("claim_type", claimLite.Type);
            Assert.Equal("claim_value", claimLite.Value);
            Assert.Equal("claim_value_type", claimLite.ValueType);
        }

        [Fact]
        public void ClaimLite_Properties_Should_Allow_Null_Values()
        {
            // Arrange
            var claimLite = new ClaimLite();

            // Assert
            Assert.Null(claimLite.Type);
            Assert.Null(claimLite.Value);
            Assert.Null(claimLite.ValueType);
        }
    }
}
