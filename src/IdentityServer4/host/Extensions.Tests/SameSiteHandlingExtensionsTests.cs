using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using IdentityServerHost.Extensions;

namespace IdentityServerHost.Extensions.Tests
{
    public class SameSiteHandlingExtensionsTests
    {
        [Fact]
        public void AddSameSiteCookiePolicy_ConfiguresServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddSameSiteCookiePolicy();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CookiePolicyOptions>>().Value;
            Assert.Equal(SameSiteMode.Unspecified, options.MinimumSameSitePolicy);
            Assert.NotNull(options.OnAppendCookie);
            Assert.NotNull(options.OnDeleteCookie);
        }

        [Theory]
        [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X)", true)]
        [InlineData("Mozilla/5.0 (iPad; CPU OS 12_0 like Mac OS X)", true)]
        [InlineData("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_0) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Safari/605.1.15", true)]
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/60.0.3112.113", true)]
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/70.0.3538.102", false)]
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/80.0.3987.149", false)]
        public void DisallowsSameSiteNone_ReturnsExpectedResult(string userAgent, bool expected)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["User-Agent"] = userAgent;
            var options = new CookieOptions { SameSite = SameSiteMode.None };

            // Act
            var services = new ServiceCollection();
            services.AddSameSiteCookiePolicy();
            var serviceProvider = services.BuildServiceProvider();
            var cookiePolicyOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CookiePolicyOptions>>().Value;
            cookiePolicyOptions.OnAppendCookie(new AppendCookieContext 
            { 
                Context = httpContext, 
                CookieOptions = options 
            });

            // Assert
            if (expected)
            {
                Assert.Equal(SameSiteMode.Unspecified, options.SameSite);
            }
            else
            {
                Assert.Equal(SameSiteMode.None, options.SameSite);
            }
        }
    }
}
