using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Pkce
{
    public class PkceAdditionalTests
    {
        private const string Category = "PKCE Additional";
        private IdentityServerPipeline _pipeline = new IdentityServerPipeline();
        private const string client_id = "code_client";
        private const string redirect_uri = "https://code_client/callback";
        private const string client_secret = "secret";

        public PkceAdditionalTests()
        {
            _pipeline.Initialize();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_CodeChallenge_Should_Fail()
        {
            await _pipeline.LoginAsync("bob");

            var authorizeResponse = await _pipeline.RequestAuthorizationEndpointAsync(
                client_id,
                "code",
                IdentityServerConstants.StandardScopes.OpenId,
                redirect_uri,
                codeChallenge: "",
                codeChallengeMethod: OidcConstants.CodeChallengeMethods.Sha256);

            _pipeline.ErrorWasCalled.Should().BeTrue();
            _pipeline.ErrorMessage.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_CodeChallengeMethod_Should_Fail()
        {
            await _pipeline.LoginAsync("bob");

            var authorizeResponse = await _pipeline.RequestAuthorizationEndpointAsync(
                client_id,
                "code",
                IdentityServerConstants.StandardScopes.OpenId,
                redirect_uri,
                codeChallenge: "challenge",
                codeChallengeMethod: "invalid_method");

            _pipeline.ErrorWasCalled.Should().BeTrue();
            _pipeline.ErrorMessage.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_CodeChallenge_With_CodeChallengeMethod_Should_Fail()
        {
            await _pipeline.LoginAsync("bob");

            var authorizeResponse = await _pipeline.RequestAuthorizationEndpointAsync(
                client_id,
                "code",
                IdentityServerConstants.StandardScopes.OpenId,
                redirect_uri,
                codeChallengeMethod: OidcConstants.CodeChallengeMethods.Sha256);

            _pipeline.ErrorWasCalled.Should().BeTrue();
            _pipeline.ErrorMessage.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task CodeChallenge_Without_CodeChallengeMethod_Should_Default_To_Plain()
        {
            await _pipeline.LoginAsync("bob");

            var authorizeResponse = await _pipeline.RequestAuthorizationEndpointAsync(
                "code_client_optional",
                "code",
                IdentityServerConstants.StandardScopes.OpenId,
                redirect_uri,
                codeChallenge: "challenge");

            authorizeResponse.IsError.Should().BeFalse();
        }
    }
}
