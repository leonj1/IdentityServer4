using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Extensions
{
    public class HttpContextExtensionsTests
    {
        private readonly HttpContext _context;
        private readonly IServiceCollection _services;

        public HttpContextExtensionsTests()
        {
            _services = new ServiceCollection();
            var serviceProvider = _services.BuildServiceProvider();
            
            _context = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
        }

        [Fact]
        public void SetIdentityServerOrigin_WhenContextIsNull_ThrowsArgumentNullException()
        {
            HttpContext context = null;
            Action act = () => context.SetIdentityServerOrigin("https://test.com");
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("context");
        }

        [Fact]
        public void SetIdentityServerOrigin_WhenValueIsNull_ThrowsArgumentNullException()
        {
            Action act = () => _context.SetIdentityServerOrigin(null);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("value");
        }

        [Fact]
        public void SetIdentityServerOrigin_WithValidValue_SetsSchemeAndHost()
        {
            _context.SetIdentityServerOrigin("https://test.com");
            
            _context.Request.Scheme.Should().Be("https");
            _context.Request.Host.Value.Should().Be("test.com");
        }

        [Fact]
        public void GetIdentityServerBaseUrl_ReturnsCorrectUrl()
        {
            _context.SetIdentityServerOrigin("https://test.com");
            _context.SetIdentityServerBasePath("/base");
            
            var result = _context.GetIdentityServerBaseUrl();
            
            result.Should().Be("https://test.com/base");
        }

        [Theory]
        [InlineData("~/path", "https://test.com/base/path")]
        [InlineData("/path", "https://test.com/base/path")]
        [InlineData("path", "https://test.com/base/path")]
        public void GetIdentityServerRelativeUrl_WithValidPath_ReturnsCorrectUrl(string path, string expected)
        {
            _context.SetIdentityServerOrigin("https://test.com");
            _context.SetIdentityServerBasePath("/base");
            
            var result = _context.GetIdentityServerRelativeUrl(path);
            
            result.Should().Be(expected);
        }

        [Fact]
        public void GetIdentityServerRelativeUrl_WithNonLocalUrl_ReturnsNull()
        {
            var result = _context.GetIdentityServerRelativeUrl("https://external.com");
            
            result.Should().BeNull();
        }
    }
}
