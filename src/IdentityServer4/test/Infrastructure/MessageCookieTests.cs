using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Infrastructure
{
    public class MessageCookieTests
    {
        private readonly ILogger<MessageCookie<TestMessage>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly MockHttpContext _mockHttpContext;
        private readonly MessageCookie<TestMessage> _subject;

        public MessageCookieTests()
        {
            _logger = Mock.Of<ILogger<MessageCookie<TestMessage>>>();
            
            _options = new IdentityServerOptions();
            _options.UserInteraction.CookieMessageThreshold = 2;

            _mockHttpContext = new MockHttpContext();
            _contextAccessor = new HttpContextAccessor { HttpContext = _mockHttpContext };
            
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
            
            _subject = new MessageCookie<TestMessage>(
                _logger,
                _options,
                _contextAccessor,
                _dataProtectionProvider);
        }

        [Fact]
        public void Write_should_create_cookie_with_message()
        {
            // Arrange
            var message = new Message<TestMessage>(new TestMessage { Value = "test" });
            var id = "123";

            // Act
            _subject.Write(id, message);

            // Assert
            _mockHttpContext.RequestCookies.Count.Should().Be(0);
            _mockHttpContext.ResponseCookies.Count.Should().Be(1);
        }

        [Fact]
        public void Read_should_return_message_from_cookie()
        {
            // Arrange
            var message = new Message<TestMessage>(new TestMessage { Value = "test" });
            var id = "123";
            _subject.Write(id, message);

            // simulate redirect
            _mockHttpContext.RequestCookies = _mockHttpContext.ResponseCookies;
            _mockHttpContext.ResponseCookies = new MockResponseCookies();

            // Act
            var result = _subject.Read(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Value.Should().Be("test");
        }

        [Fact]
        public void Clear_should_remove_cookie()
        {
            // Arrange
            var message = new Message<TestMessage>(new TestMessage { Value = "test" });
            var id = "123";
            _subject.Write(id, message);

            // Act
            _subject.Clear(id);

            // Assert
            _mockHttpContext.ResponseCookies.Cookies.First().Expires.Should().BeBefore(DateTime.Now);
        }

        private class TestMessage
        {
            public string Value { get; set; }
        }

        private class MockHttpContext : DefaultHttpContext
        {
            public MockRequestCookies RequestCookies { get; set; } = new MockRequestCookies();
            public MockResponseCookies ResponseCookies { get; set; } = new MockResponseCookies();

            public override IRequestCookieCollection Cookies => RequestCookies;
            public override IResponseCookies Response { get { return ResponseCookies; } }
        }

        private class MockRequestCookies : Dictionary<string, string>, IRequestCookieCollection
        {
            public new string this[string key] => TryGetValue(key, out string value) ? value : null;
            public new ICollection<string> Keys => base.Keys;
        }

        private class MockResponseCookies : IResponseCookies
        {
            public List<(string Key, string Value, CookieOptions Options)> Cookies { get; set; } = new List<(string Key, string Value, CookieOptions Options)>();

            public void Append(string key, string value, CookieOptions options)
            {
                Cookies.Add((key, value, options));
            }

            public void Delete(string key, CookieOptions options)
            {
                Cookies.Add((key, null, options));
            }
        }
    }
}
