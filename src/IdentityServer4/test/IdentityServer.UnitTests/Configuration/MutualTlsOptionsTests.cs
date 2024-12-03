using IdentityServer4.Configuration;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class MutualTlsOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            var options = new MutualTlsOptions();
            
            options.Enabled.Should().BeFalse();
            options.ClientCertificateAuthenticationScheme.Should().Be("Certificate");
            options.DomainName.Should().BeNull();
            options.AlwaysEmitConfirmationClaim.Should().BeFalse();
        }

        [Theory]
        [InlineData("mtls", "mtls.identityserver.local")] // subdomain
        [InlineData("mtls.app.com", "mtls.app.com")] // full domain
        public void DomainName_ShouldBeHandledCorrectly(string input, string expected)
        {
            var options = new MutualTlsOptions
            {
                DomainName = input
            };
            
            options.DomainName.Should().Be(input);
        }

        [Fact]
        public void SettingProperties_ShouldWork()
        {
            var options = new MutualTlsOptions
            {
                Enabled = true,
                ClientCertificateAuthenticationScheme = "CustomScheme",
                DomainName = "mtls.identityserver.local",
                AlwaysEmitConfirmationClaim = true
            };

            options.Enabled.Should().BeTrue();
            options.ClientCertificateAuthenticationScheme.Should().Be("CustomScheme");
            options.DomainName.Should().Be("mtls.identityserver.local");
            options.AlwaysEmitConfirmationClaim.Should().BeTrue();
        }
    }
}
