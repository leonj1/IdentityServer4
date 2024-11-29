// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer.IntegrationTests.Clients.Setup;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class DiscoveryClientTests
    {
        private const string DiscoveryEndpoint = "https://server/.well-known/openid-configuration";

        private readonly HttpClient _client;

        public DiscoveryClientTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _client = server.CreateClient();
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_values()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy =
                {
                    ValidateIssuerName = false
                }
            });

            // endpoints
            doc.TokenEndpoint.Should().Be("https://server/connect/token");
            doc.AuthorizeEndpoint.Should().Be("https://server/connect/authorize");
            doc.IntrospectionEndpoint.Should().Be("https://server/connect/introspect");
            doc.EndSessionEndpoint.Should().Be("https://server/connect/endsession");

            // jwk
            doc.KeySet.Keys.Count.Should().Be(1);
            doc.KeySet.Keys.First().E.Should().NotBeNull();
            doc.KeySet.Keys.First().N.Should().NotBeNull();
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_grant_types()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy = { ValidateIssuerName = false }
            });

            doc.GrantTypesSupported.Should().Contain("authorization_code");
            doc.GrantTypesSupported.Should().Contain("client_credentials");
            doc.GrantTypesSupported.Should().Contain("password");
            doc.GrantTypesSupported.Should().Contain("implicit");
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_scopes()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy = { ValidateIssuerName = false }
            });

            doc.ScopesSupported.Should().Contain("openid");
            doc.ScopesSupported.Should().Contain("profile");
            doc.ScopesSupported.Should().Contain("email");
        }

        [Fact]
        public async Task Invalid_discovery_endpoint_should_fail()
        {
            var response = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://invalid-server/.well-known/openid-configuration",
                Policy = { ValidateIssuerName = false }
            });

            response.IsError.Should().BeTrue();
            response.Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_response_types()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy = { ValidateIssuerName = false }
            });

            doc.ResponseTypesSupported.Should().Contain("code");
            doc.ResponseTypesSupported.Should().Contain("token");
            doc.ResponseTypesSupported.Should().Contain("id_token");
        }
    }
}
