using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class CspOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var options = new CspOptions();

            // Assert
            Assert.Equal(CspLevel.Two, options.Level);
            Assert.True(options.AddDeprecatedHeader);
        }

        [Fact]
        public void SettingValues_ShouldWork()
        {
            // Arrange
            var options = new CspOptions
            {
                Level = CspLevel.One,
                AddDeprecatedHeader = false
            };

            // Assert
            Assert.Equal(CspLevel.One, options.Level);
            Assert.False(options.AddDeprecatedHeader);
        }
    }
}
