using IdentityModel;
using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class LoggingOptionsTests
    {
        [Fact]
        public void TokenRequestSensitiveValuesFilter_ShouldHaveDefaultValues()
        {
            // Arrange
            var options = new LoggingOptions();

            // Assert
            Assert.Contains(OidcConstants.TokenRequest.ClientSecret, options.TokenRequestSensitiveValuesFilter);
            Assert.Contains(OidcConstants.TokenRequest.Password, options.TokenRequestSensitiveValuesFilter);
            Assert.Contains(OidcConstants.TokenRequest.ClientAssertion, options.TokenRequestSensitiveValuesFilter);
            Assert.Contains(OidcConstants.TokenRequest.RefreshToken, options.TokenRequestSensitiveValuesFilter);
            Assert.Contains(OidcConstants.TokenRequest.DeviceCode, options.TokenRequestSensitiveValuesFilter);
        }

        [Fact]
        public void AuthorizeRequestSensitiveValuesFilter_ShouldHaveDefaultValues()
        {
            // Arrange
            var options = new LoggingOptions();

            // Assert
            Assert.Contains(OidcConstants.AuthorizeRequest.IdTokenHint, options.AuthorizeRequestSensitiveValuesFilter);
        }

        [Fact]
        public void TokenRequestSensitiveValuesFilter_CanBeModified()
        {
            // Arrange
            var options = new LoggingOptions();
            var newValue = "new_sensitive_value";

            // Act
            options.TokenRequestSensitiveValuesFilter.Add(newValue);

            // Assert
            Assert.Contains(newValue, options.TokenRequestSensitiveValuesFilter);
        }

        [Fact]
        public void AuthorizeRequestSensitiveValuesFilter_CanBeModified()
        {
            // Arrange
            var options = new LoggingOptions();
            var newValue = "new_sensitive_value";

            // Act
            options.AuthorizeRequestSensitiveValuesFilter.Add(newValue);

            // Assert
            Assert.Contains(newValue, options.AuthorizeRequestSensitiveValuesFilter);
        }
    }
}
