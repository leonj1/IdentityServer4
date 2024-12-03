using IdentityServer4;
using Xunit;

namespace IdentityServer4.UnitTests
{
    public class IdentityServerConstantsTests
    {
        [Fact]
        public void ProtocolTypes_Should_Have_Expected_Values()
        {
            Assert.Equal("oidc", IdentityServerConstants.ProtocolTypes.OpenIdConnect);
        }

        [Fact]
        public void SecretTypes_Should_Have_Expected_Values()
        {
            Assert.Equal("SharedSecret", IdentityServerConstants.SecretTypes.SharedSecret);
        }

        [Fact]
        public void StandardScopes_Should_Have_Expected_Values()
        {
            Assert.Equal("openid", IdentityServerConstants.StandardScopes.OpenId);
            Assert.Equal("profile", IdentityServerConstants.StandardScopes.Profile);
            Assert.Equal("email", IdentityServerConstants.StandardScopes.Email);
            Assert.Equal("address", IdentityServerConstants.StandardScopes.Address);
            Assert.Equal("phone", IdentityServerConstants.StandardScopes.Phone);
            Assert.Equal("offline_access", IdentityServerConstants.StandardScopes.OfflineAccess);
        }

        [Fact]
        public void PersistedGrantTypes_Should_Have_Expected_Values()
        {
            Assert.Equal("authorization_code", IdentityServerConstants.PersistedGrantTypes.AuthorizationCode);
            Assert.Equal("reference_token", IdentityServerConstants.PersistedGrantTypes.ReferenceToken);
            Assert.Equal("refresh_token", IdentityServerConstants.PersistedGrantTypes.RefreshToken);
            Assert.Equal("user_consent", IdentityServerConstants.PersistedGrantTypes.UserConsent);
            Assert.Equal("device_code", IdentityServerConstants.PersistedGrantTypes.DeviceCode);
            Assert.Equal("user_code", IdentityServerConstants.PersistedGrantTypes.UserCode);
        }
    }
}
