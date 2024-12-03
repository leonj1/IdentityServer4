using IdentityServer4.Models;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class DiscoveryDocumentTests
    {
        [Fact]
        public void DiscoveryDocument_Properties_Should_Be_Settable()
        {
            // Arrange
            var document = new DiscoveryDocument
            {
                issuer = "https://demo.identityserver.io",
                jwks_uri = "https://demo.identityserver.io/.well-known/jwks",
                authorization_endpoint = "https://demo.identityserver.io/connect/authorize",
                token_endpoint = "https://demo.identityserver.io/connect/token",
                userinfo_endpoint = "https://demo.identityserver.io/connect/userinfo",
                end_session_endpoint = "https://demo.identityserver.io/connect/endsession",
                check_session_iframe = "https://demo.identityserver.io/connect/checksession",
                revocation_endpoint = "https://demo.identityserver.io/connect/revocation",
                introspection_endpoint = "https://demo.identityserver.io/connect/introspect",
                frontchannel_logout_supported = true,
                frontchannel_logout_session_supported = true,
                scopes_supported = new[] { "openid", "profile", "email" },
                claims_supported = new[] { "sub", "name", "email" },
                response_types_supported = new[] { "code", "token", "id_token" },
                response_modes_supported = new[] { "form_post", "query", "fragment" },
                grant_types_supported = new[] { "authorization_code", "client_credentials" },
                subject_types_supported = new[] { "public" },
                id_token_signing_alg_values_supported = new[] { "RS256" },
                token_endpoint_auth_methods_supported = new[] { "client_secret_basic" },
                code_challenge_methods_supported = new[] { "S256" }
            };

            // Assert
            document.issuer.Should().Be("https://demo.identityserver.io");
            document.jwks_uri.Should().Be("https://demo.identityserver.io/.well-known/jwks");
            document.authorization_endpoint.Should().Be("https://demo.identityserver.io/connect/authorize");
            document.token_endpoint.Should().Be("https://demo.identityserver.io/connect/token");
            document.userinfo_endpoint.Should().Be("https://demo.identityserver.io/connect/userinfo");
            document.end_session_endpoint.Should().Be("https://demo.identityserver.io/connect/endsession");
            document.check_session_iframe.Should().Be("https://demo.identityserver.io/connect/checksession");
            document.revocation_endpoint.Should().Be("https://demo.identityserver.io/connect/revocation");
            document.introspection_endpoint.Should().Be("https://demo.identityserver.io/connect/introspect");
            document.frontchannel_logout_supported.Should().BeTrue();
            document.frontchannel_logout_session_supported.Should().BeTrue();
            document.scopes_supported.Should().BeEquivalentTo(new[] { "openid", "profile", "email" });
            document.claims_supported.Should().BeEquivalentTo(new[] { "sub", "name", "email" });
            document.response_types_supported.Should().BeEquivalentTo(new[] { "code", "token", "id_token" });
            document.response_modes_supported.Should().BeEquivalentTo(new[] { "form_post", "query", "fragment" });
            document.grant_types_supported.Should().BeEquivalentTo(new[] { "authorization_code", "client_credentials" });
            document.subject_types_supported.Should().BeEquivalentTo(new[] { "public" });
            document.id_token_signing_alg_values_supported.Should().BeEquivalentTo(new[] { "RS256" });
            document.token_endpoint_auth_methods_supported.Should().BeEquivalentTo(new[] { "client_secret_basic" });
            document.code_challenge_methods_supported.Should().BeEquivalentTo(new[] { "S256" });
        }

        [Fact]
        public void DiscoveryDocument_Should_Handle_Null_Values()
        {
            // Arrange
            var document = new DiscoveryDocument();

            // Assert
            document.issuer.Should().BeNull();
            document.jwks_uri.Should().BeNull();
            document.authorization_endpoint.Should().BeNull();
            document.token_endpoint.Should().BeNull();
            document.userinfo_endpoint.Should().BeNull();
            document.end_session_endpoint.Should().BeNull();
            document.check_session_iframe.Should().BeNull();
            document.revocation_endpoint.Should().BeNull();
            document.introspection_endpoint.Should().BeNull();
            document.frontchannel_logout_supported.Should().BeNull();
            document.frontchannel_logout_session_supported.Should().BeNull();
            document.scopes_supported.Should().BeNull();
            document.claims_supported.Should().BeNull();
            document.response_types_supported.Should().BeNull();
            document.response_modes_supported.Should().BeNull();
            document.grant_types_supported.Should().BeNull();
            document.subject_types_supported.Should().BeNull();
            document.id_token_signing_alg_values_supported.Should().BeNull();
            document.token_endpoint_auth_methods_supported.Should().BeNull();
            document.code_challenge_methods_supported.Should().BeNull();
        }
    }
}
