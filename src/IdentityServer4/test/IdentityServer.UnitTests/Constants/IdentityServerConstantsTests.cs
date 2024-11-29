using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Xunit;

namespace IdentityServer.UnitTests.Constants
{
    public class IdentityServerConstantsTests
    {
        [Fact]
        public void StandardScopeConstants_Should_Have_Correct_Values()
        {
            Assert.Equal("openid", IdentityServer4.IdentityServerConstants.StandardScopes.OpenId);
            Assert.Equal("profile", IdentityServer4.IdentityServerConstants.StandardScopes.Profile);
            Assert.Equal("email", IdentityServer4.IdentityServerConstants.StandardScopes.Email);
            Assert.Equal("address", IdentityServer4.IdentityServerConstants.StandardScopes.Address);
            Assert.Equal("phone", IdentityServer4.IdentityServerConstants.StandardScopes.Phone);
            Assert.Equal("offline_access", IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
        }

        [Fact]
        public void SupportedSigningAlgorithms_Should_Contain_Expected_Algorithms()
        {
            var algorithms = IdentityServer4.IdentityServerConstants.SupportedSigningAlgorithms;
            
            Assert.Contains(SecurityAlgorithms.RsaSha256, algorithms);
            Assert.Contains(SecurityAlgorithms.RsaSha384, algorithms);
            Assert.Contains(SecurityAlgorithms.RsaSha512, algorithms);
            Assert.Contains(SecurityAlgorithms.RsaSsaPssSha256, algorithms);
            Assert.Contains(SecurityAlgorithms.RsaSsaPssSha384, algorithms);
            Assert.Contains(SecurityAlgorithms.RsaSsaPssSha512, algorithms);
            Assert.Contains(SecurityAlgorithms.EcdsaSha256, algorithms);
            Assert.Contains(SecurityAlgorithms.EcdsaSha384, algorithms);
            Assert.Contains(SecurityAlgorithms.EcdsaSha512, algorithms);
            
            Assert.Equal(9, algorithms.Count());
        }

        [Fact]
        public void LocalApi_Constants_Should_Have_Correct_Values()
        {
            Assert.Equal("IdentityServerAccessToken", IdentityServer4.IdentityServerConstants.LocalApi.AuthenticationScheme);
            Assert.Equal("IdentityServerApi", IdentityServer4.IdentityServerConstants.LocalApi.ScopeName);
            Assert.Equal(IdentityServer4.IdentityServerConstants.LocalApi.AuthenticationScheme, 
                        IdentityServer4.IdentityServerConstants.LocalApi.PolicyName);
        }

        [Fact]
        public void ProtocolTypes_Should_Have_Correct_Values()
        {
            Assert.Equal("oidc", IdentityServer4.IdentityServerConstants.ProtocolTypes.OpenIdConnect);
            Assert.Equal("wsfed", IdentityServer4.IdentityServerConstants.ProtocolTypes.WsFederation);
            Assert.Equal("saml2p", IdentityServer4.IdentityServerConstants.ProtocolTypes.Saml2p);
        }

        [Fact]
        public void HttpClients_Should_Have_Correct_Values()
        {
            Assert.Equal(10, IdentityServer4.IdentityServerConstants.HttpClients.DefaultTimeoutSeconds);
            Assert.Equal("IdentityServer:JwtRequestUriClient", 
                        IdentityServer4.IdentityServerConstants.HttpClients.JwtRequestClientKey);
            Assert.Equal("IdentityServer:BackChannelLogoutClient", 
                        IdentityServer4.IdentityServerConstants.HttpClients.BackChannelLogoutHttpClient);
        }
    }
}
