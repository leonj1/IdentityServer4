using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Net.Http.Headers;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultJwtRequestUriHttpClientTests
    {
        private readonly IdentityServerOptions _options;
        private readonly MockMessageHandler _handler;
        private readonly DefaultJwtRequestUriHttpClient _subject;
        private readonly Client _client;

        public DefaultJwtRequestUriHttpClientTests()
        {
            _options = new IdentityServerOptions();
            _handler = new MockMessageHandler();
            
            var client = new HttpClient(_handler);
            _subject = new DefaultJwtRequestUriHttpClient(
                client,
                _options,
                new LoggerFactory());

            _client = new Client();
        }

        [Fact]
        public async Task valid_response_should_return_content()
        {
            _handler.Response.StatusCode = HttpStatusCode.OK;
            _handler.Response.Content = new StringContent("jwt_content");
            _handler.Response.Content.Headers.ContentType = 
                new MediaTypeHeaderValue($"application/{JwtClaimTypes.JwtTypes.AuthorizationRequest}");

            var response = await _subject.GetJwtAsync("http://test", _client);

            response.Should().Be("jwt_content");
        }

        [Fact]
        public async Task invalid_content_type_should_return_null_when_strict_validation()
        {
            _options.StrictJarValidation = true;
            
            _handler.Response.StatusCode = HttpStatusCode.OK;
            _handler.Response.Content = new StringContent("jwt_content");
            _handler.Response.Content.Headers.ContentType = 
                new MediaTypeHeaderValue("application/json");

            var response = await _subject.GetJwtAsync("http://test", _client);

            response.Should().BeNull();
        }

        [Fact]
        public async Task non_success_response_should_return_null()
        {
            _handler.Response.StatusCode = HttpStatusCode.NotFound;

            var response = await _subject.GetJwtAsync("http://test", _client);

            response.Should().BeNull();
        }
    }

    public class MockMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }
    }
}
