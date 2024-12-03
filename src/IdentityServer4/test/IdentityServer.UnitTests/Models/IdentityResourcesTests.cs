using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using System.Linq;
using Xunit;

namespace IdentityServer.UnitTests.Models
{
    public class IdentityResourcesTests
    {
        [Fact]
        public void OpenId_ShouldHaveCorrectDefaultValues()
        {
            var openId = new IdentityResources.OpenId();

            openId.Name.Should().Be(IdentityServerConstants.StandardScopes.OpenId);
            openId.DisplayName.Should().Be("Your user identifier");
            openId.Required.Should().BeTrue();
            openId.UserClaims.Should().Contain(JwtClaimTypes.Subject);
        }

        [Fact]
        public void Profile_ShouldHaveCorrectDefaultValues()
        {
            var profile = new IdentityResources.Profile();

            profile.Name.Should().Be(IdentityServerConstants.StandardScopes.Profile);
            profile.DisplayName.Should().Be("User profile");
            profile.Description.Should().Be("Your user profile information (first name, last name, etc.)");
            profile.Emphasize.Should().BeTrue();
            profile.UserClaims.Should().BeEquivalentTo(
                Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Profile]);
        }

        [Fact]
        public void Email_ShouldHaveCorrectDefaultValues()
        {
            var email = new IdentityResources.Email();

            email.Name.Should().Be(IdentityServerConstants.StandardScopes.Email);
            email.DisplayName.Should().Be("Your email address");
            email.Emphasize.Should().BeTrue();
            email.UserClaims.Should().BeEquivalentTo(
                Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Email]);
        }

        [Fact]
        public void Phone_ShouldHaveCorrectDefaultValues()
        {
            var phone = new IdentityResources.Phone();

            phone.Name.Should().Be(IdentityServerConstants.StandardScopes.Phone);
            phone.DisplayName.Should().Be("Your phone number");
            phone.Emphasize.Should().BeTrue();
            phone.UserClaims.Should().BeEquivalentTo(
                Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Phone]);
        }

        [Fact]
        public void Address_ShouldHaveCorrectDefaultValues()
        {
            var address = new IdentityResources.Address();

            address.Name.Should().Be(IdentityServerConstants.StandardScopes.Address);
            address.DisplayName.Should().Be("Your postal address");
            address.Emphasize.Should().BeTrue();
            address.UserClaims.Should().BeEquivalentTo(
                Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Address]);
        }
    }
}
