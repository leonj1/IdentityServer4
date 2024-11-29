using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Hosting
{
    public class CorsPolicyProviderTests
    {
        private readonly ILogger<CorsPolicyProvider> _logger;
        private readonly IdentityServerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;
        private readonly MockCorsPolicyService _mockCorsPolicyService;
        private readonly MockInnerCorsPolicyProvider _mockInnerProvider;

        public CorsPolicyProviderTests()
        {
            _logger = new LoggerFactory().CreateLogger<CorsPolicyProvider>();
            
            _options = new IdentityServerOptions();
            _options.Cors.CorsPolicyName = "IdentityServer";
            _options.Cors.CorsPaths = new[] { "/connect/token", "/connect/userinfo" };
            
            _httpContext = new DefaultHttpContext();
            _httpContextAccessor = new HttpContextAccessor { HttpContext = _httpContext };
            
            _mockCorsPolicyService = new MockCorsPolicyService();
            var services = new ServiceCollection();
            services.AddTransient<ICorsPolicyService>(_ => _mockCorsPolicyService);
            _httpContext.RequestServices = services.BuildServiceProvider();
            
            _mockInnerProvider = new MockInnerCorsPolicyProvider();
        }

        [Fact]
        public async Task When_Path_Not_Allowed_Should_Return_Null()
        {
            // Arrange
            var provider = new CorsPolicyProvider(
                _logger,
                new Decorator<ICorsPolicyProvider>(_mockInnerProvider),
                _options,
                _httpContextAccessor);

            _httpContext.Request.Path = "/not-allowed";
            _httpContext.Request.Headers.Add("Origin", "https://allowed-origin.com");

            // Act
            var result = await provider.GetPolicyAsync(_httpContext, "IdentityServer");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task When_Origin_Not_Allowed_Should_Return_Null()
        {
            // Arrange
            var provider = new CorsPolicyProvider(
                _logger,
                new Decorator<ICorsPolicyProvider>(_mockInnerProvider),
                _options,
                _httpContextAccessor);

            _httpContext.Request.Path = "/connect/token";
            _httpContext.Request.Headers.Add("Origin", "https://not-allowed-origin.com");
            _mockCorsPolicyService.Response = false;

            // Act
            var result = await provider.GetPolicyAsync(_httpContext, "IdentityServer");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task When_Origin_And_Path_Allowed_Should_Return_Policy()
        {
            // Arrange
            var provider = new CorsPolicyProvider(
                _logger,
                new Decorator<ICorsPolicyProvider>(_mockInnerProvider),
                _options,
                _httpContextAccessor);

            _httpContext.Request.Path = "/connect/token";
            _httpContext.Request.Headers.Add("Origin", "https://allowed-origin.com");
            _mockCorsPolicyService.Response = true;

            // Act
            var result = await provider.GetPolicyAsync(_httpContext, "IdentityServer");

            // Assert
            result.Should().NotBeNull();
            result.Origins.Should().Contain("https://allowed-origin.com");
            result.AllowAnyHeader.Should().BeTrue();
            result.AllowAnyMethod.Should().BeTrue();
        }

        [Fact]
        public async Task When_Different_PolicyName_Should_Use_Inner_Provider()
        {
            // Arrange
            var provider = new CorsPolicyProvider(
                _logger,
                new Decorator<ICorsPolicyProvider>(_mockInnerProvider),
                _options,
                _httpContextAccessor);

            // Act
            var result = await provider.GetPolicyAsync(_httpContext, "DifferentPolicy");

            // Assert
            _mockInnerProvider.WasCalled.Should().BeTrue();
        }

        private class MockCorsPolicyService : ICorsPolicyService
        {
            public bool Response { get; set; }
            
            public Task<bool> IsOriginAllowedAsync(string origin)
            {
                return Task.FromResult(Response);
            }
        }

        private class MockInnerCorsPolicyProvider : ICorsPolicyProvider
        {
            public bool WasCalled { get; private set; }

            public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
            {
                WasCalled = true;
                return Task.FromResult<CorsPolicy>(null);
            }
        }
    }
}
