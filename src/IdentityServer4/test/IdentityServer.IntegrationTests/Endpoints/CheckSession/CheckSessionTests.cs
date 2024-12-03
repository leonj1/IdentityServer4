// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.CheckSession
{
    public class CheckSessionTests
    {
        private const string Category = "Check session endpoint";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        public CheckSessionTests()
        {
            _mockPipeline.Initialize();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task get_request_should_not_return_404()
        {
            var response = await _mockPipeline.BackChannelClient.GetAsync(IdentityServerPipeline.CheckSessionEndpoint);

            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task post_request_should_return_405()
        {
            var response = await _mockPipeline.BackChannelClient.PostAsync(IdentityServerPipeline.CheckSessionEndpoint, null);

            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task get_request_should_return_html_content()
        {
            var response = await _mockPipeline.BackChannelClient.GetAsync(IdentityServerPipeline.CheckSessionEndpoint);

            response.Content.Headers.ContentType.MediaType.Should().Be("text/html");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task get_request_should_return_success_status()
        {
            var response = await _mockPipeline.BackChannelClient.GetAsync(IdentityServerPipeline.CheckSessionEndpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task get_request_should_include_cache_control_headers()
        {
            var response = await _mockPipeline.BackChannelClient.GetAsync(IdentityServerPipeline.CheckSessionEndpoint);

            response.Headers.CacheControl.NoCache.Should().BeTrue();
            response.Headers.CacheControl.NoStore.Should().BeTrue();
        }
    }
}
