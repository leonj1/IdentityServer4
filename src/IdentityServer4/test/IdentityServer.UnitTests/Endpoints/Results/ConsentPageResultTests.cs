using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;
using static IdentityServer4.Constants;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class ConsentPageResultTests
    {
        private ConsentPageResult _subject;
        private ValidatedAuthorizeRequest _request;
        private IdentityServerOptions _options;
        private MockMessageStore<IDictionary<string, string[]>> _mockAuthorizationParametersMessageStore;
        private HttpContext _context;

        public ConsentPageResultTests()
        {
            _request = new ValidatedAuthorizeRequest()
            {
                Raw = new Dictionary<string, string[]>
                {
                    { "client_id", new[] { "client" } },
                    { "scope", new[] { "scope1 scope2" } }
                }
            };

            _options = new IdentityServerOptions();
            _options.UserInteraction.ConsentUrl = "/consent";
            _options.UserInteraction.ConsentReturnUrlParameter = "returnUrl";

            _mockAuthorizationParametersMessageStore = new MockMessageStore<IDictionary<string, string[]>>();

            _context = new DefaultHttpContext();
            _context.SetIdentityServerBasePath("/");
            _context.Request.Scheme = "https";
            _context.Request.Host = new HostString("server");

            _subject = new ConsentPageResult(_request, _options, _mockAuthorizationParametersMessageStore);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRedirectToConsentPage()
        {
            await _subject.ExecuteAsync(_context);

            _context.Response.StatusCode.Should().Be(302);
            _context.Response.Headers["Location"].First().Should().StartWith("/consent");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIncludeReturnUrlParameter()
        {
            await _subject.ExecuteAsync(_context);

            var location = _context.Response.Headers["Location"].First();
            location.Should().Contain("returnUrl=");
        }

        [Fact]
        public async Task ExecuteAsync_WhenMessageStorePresent_ShouldStoreParameters()
        {
            await _subject.ExecuteAsync(_context);

            _mockAuthorizationParametersMessageStore.Messages.Count.Should().Be(1);
        }

        [Fact]
        public void Constructor_NullRequest_ThrowsException()
        {
            Action act = () => new ConsentPageResult(null);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [Fact]
        public async Task ExecuteAsync_AbsoluteConsentUrl_ShouldIncludeAbsoluteReturnUrl()
        {
            _options.UserInteraction.ConsentUrl = "https://external/consent";

            await _subject.ExecuteAsync(_context);

            var location = _context.Response.Headers["Location"].First();
            location.Should().StartWith("https://external/consent");
            location.Should().Contain("https://server/");
        }
    }

    internal class MockMessageStore<T> : IAuthorizationParametersMessageStore
    {
        public Dictionary<string, Message<IDictionary<string, string[]>>> Messages { get; set; } = new Dictionary<string, Message<IDictionary<string, string[]>>>();

        public Task<string> WriteAsync(Message<IDictionary<string, string[]>> message)
        {
            var id = Guid.NewGuid().ToString();
            Messages.Add(id, message);
            return Task.FromResult(id);
        }

        public Task<Message<IDictionary<string, string[]>>> ReadAsync(string id)
        {
            Messages.TryGetValue(id, out var msg);
            return Task.FromResult(msg);
        }

        public Task DeleteAsync(string id)
        {
            Messages.Remove(id);
            return Task.CompletedTask;
        }
    }
}
