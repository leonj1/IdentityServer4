using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Pipeline
{
    public class CorsTestsAdditional
    {
        private const string Category = "CORS Integration Additional";
        private IdentityServerPipeline _pipeline = new IdentityServerPipeline();

        public CorsTestsAdditional()
        {
            _pipeline.Initialize();
        }

        [Theory]
        [InlineData("https://unauthorized-client")]
        [InlineData("http://client")]  // Wrong protocol
        [InlineData("https://client.evil.com")]
        [Trait("Category", Category)]
        public async Task cors_request_from_unauthorized_origins_should_fail(string origin)
        {
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Origin", origin);
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Access-Control-Request-Method", "GET");

            var message = new HttpRequestMessage(HttpMethod.Options, IdentityServerPipeline.DiscoveryEndpoint);
            var response = await _pipeline.BackChannelClient.SendAsync(message);

            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [Trait("Category", Category)]
        public async Task cors_request_with_disallowed_methods_should_fail(string method)
        {
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Origin", "https://client");
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Access-Control-Request-Method", method);

            var message = new HttpRequestMessage(HttpMethod.Options, IdentityServerPipeline.DiscoveryEndpoint);
            var response = await _pipeline.BackChannelClient.SendAsync(message);

            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task cors_request_without_origin_header_should_fail()
        {
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Access-Control-Request-Method", "GET");

            var message = new HttpRequestMessage(HttpMethod.Options, IdentityServerPipeline.DiscoveryEndpoint);
            var response = await _pipeline.BackChannelClient.SendAsync(message);

            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task cors_request_without_method_header_should_fail()
        {
            _pipeline.BackChannelClient.DefaultRequestHeaders.Add("Origin", "https://client");

            var message = new HttpRequestMessage(HttpMethod.Options, IdentityServerPipeline.DiscoveryEndpoint);
            var response = await _pipeline.BackChannelClient.SendAsync(message);

            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }
    }
}
