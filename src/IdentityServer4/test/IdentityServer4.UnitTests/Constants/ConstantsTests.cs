using FluentAssertions;
using IdentityModel;
using Xunit;
using System.Linq;

namespace IdentityServer4.UnitTests.Constants
{
    public class ConstantsTests
    {
        [Fact]
        public void StandardConstants_Should_Have_Correct_Values()
        {
            Constants.IdentityServerName.Should().Be("IdentityServer4");
            Constants.IdentityServerAuthenticationType.Should().Be("IdentityServer4");
            Constants.DefaultHashAlgorithm.Should().Be("SHA256");
        }

        [Fact]
        public void SupportedResponseTypes_Should_Contain_All_Defined_Types()
        {
            Constants.SupportedResponseTypes.Should().Contain(OidcConstants.ResponseTypes.Code);
            Constants.SupportedResponseTypes.Should().Contain(OidcConstants.ResponseTypes.Token);
            Constants.SupportedResponseTypes.Should().Contain(OidcConstants.ResponseTypes.IdToken);
            Constants.SupportedResponseTypes.Should().HaveCount(7);
        }

        [Fact]
        public void ResponseTypeToGrantTypeMapping_Should_Be_Valid()
        {
            Constants.ResponseTypeToGrantTypeMapping[OidcConstants.ResponseTypes.Code]
                .Should().Be(GrantType.AuthorizationCode);
            
            Constants.ResponseTypeToGrantTypeMapping[OidcConstants.ResponseTypes.Token]
                .Should().Be(GrantType.Implicit);
        }

        [Fact]
        public void SupportedCodeChallengeMethods_Should_Include_Plain_And_Sha256()
        {
            Constants.SupportedCodeChallengeMethods.Should().Contain(OidcConstants.CodeChallengeMethods.Plain);
            Constants.SupportedCodeChallengeMethods.Should().Contain(OidcConstants.CodeChallengeMethods.Sha256);
            Constants.SupportedCodeChallengeMethods.Should().HaveCount(2);
        }

        [Fact]
        public void StandardScopes_Should_Have_Correct_Claims_Mapping()
        {
            var profileClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Profile];
            profileClaims.Should().Contain(JwtClaimTypes.Name);
            profileClaims.Should().Contain(JwtClaimTypes.FamilyName);
            profileClaims.Should().Contain(JwtClaimTypes.GivenName);

            var emailClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Email];
            emailClaims.Should().Contain(JwtClaimTypes.Email);
            emailClaims.Should().Contain(JwtClaimTypes.EmailVerified);
        }

        [Fact]
        public void ProtocolRoutePaths_Should_Have_Correct_Prefixes()
        {
            Constants.ProtocolRoutePaths.ConnectPathPrefix.Should().Be("connect");
            Constants.ProtocolRoutePaths.Authorize.Should().StartWith(Constants.ProtocolRoutePaths.ConnectPathPrefix);
            Constants.ProtocolRoutePaths.Token.Should().StartWith(Constants.ProtocolRoutePaths.ConnectPathPrefix);
        }
    }
}
