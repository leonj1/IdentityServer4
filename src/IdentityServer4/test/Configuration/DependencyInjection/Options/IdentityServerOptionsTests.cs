using IdentityServer4.Configuration;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class IdentityServerOptionsTests
    {
        [Fact]
        public void DefaultConstructorSetsDefaultValues()
        {
            // Arrange & Act
            var options = new IdentityServerOptions();

            // Assert
            options.IssuerUri.Should().BeNull();
            options.LowerCaseIssuerUri.Should().BeTrue();
            options.AccessTokenJwtType.Should().Be("at+jwt");
            options.EmitStaticAudienceClaim.Should().BeFalse();
            options.EmitScopesAsSpaceDelimitedStringInJwt.Should().BeFalse();
            options.StrictJarValidation.Should().BeFalse();

            // Verify default instances are created
            options.Endpoints.Should().NotBeNull();
            options.Discovery.Should().NotBeNull();
            options.Authentication.Should().NotBeNull();
            options.Events.Should().NotBeNull();
            options.InputLengthRestrictions.Should().NotBeNull();
            options.UserInteraction.Should().NotBeNull();
            options.Caching.Should().NotBeNull();
            options.Cors.Should().NotBeNull();
            options.Csp.Should().NotBeNull();
            options.Validation.Should().NotBeNull();
            options.DeviceFlow.Should().NotBeNull();
            options.Logging.Should().NotBeNull();
            options.MutualTls.Should().NotBeNull();
        }

        [Fact]
        public void CanModifyProperties()
        {
            // Arrange
            var options = new IdentityServerOptions();

            // Act
            options.IssuerUri = "https://test.com";
            options.LowerCaseIssuerUri = false;
            options.AccessTokenJwtType = "custom+jwt";
            options.EmitStaticAudienceClaim = true;
            options.EmitScopesAsSpaceDelimitedStringInJwt = true;
            options.StrictJarValidation = true;

            // Assert
            options.IssuerUri.Should().Be("https://test.com");
            options.LowerCaseIssuerUri.Should().BeFalse();
            options.AccessTokenJwtType.Should().Be("custom+jwt");
            options.EmitStaticAudienceClaim.Should().BeTrue();
            options.EmitScopesAsSpaceDelimitedStringInJwt.Should().BeTrue();
            options.StrictJarValidation.Should().BeTrue();
        }
    }
}
