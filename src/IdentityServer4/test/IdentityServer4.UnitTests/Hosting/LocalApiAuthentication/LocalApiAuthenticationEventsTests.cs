using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting.LocalApiAuthentication
{
    public class LocalApiAuthenticationEventsTests
    {
        [Fact]
        public async Task ClaimsTransformation_Should_Execute_OnClaimsTransformation()
        {
            // Arrange
            var wasExecuted = false;
            var events = new LocalApiAuthenticationEvents
            {
                OnClaimsTransformation = context =>
                {
                    wasExecuted = true;
                    return Task.CompletedTask;
                }
            };

            var context = new ClaimsTransformationContext
            {
                Principal = new ClaimsPrincipal(),
                HttpContext = new DefaultHttpContext()
            };

            // Act
            await events.ClaimsTransformation(context);

            // Assert
            wasExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Default_OnClaimsTransformation_Should_Return_CompletedTask()
        {
            // Arrange
            var events = new LocalApiAuthenticationEvents();
            var context = new ClaimsTransformationContext
            {
                Principal = new ClaimsPrincipal(),
                HttpContext = new DefaultHttpContext()
            };

            // Act
            await events.ClaimsTransformation(context);

            // Assert
            events.OnClaimsTransformation.Should().NotBeNull();
        }

        [Fact]
        public void ClaimsTransformationContext_Properties_Should_Be_Settable()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var httpContext = new DefaultHttpContext();
            var context = new ClaimsTransformationContext();

            // Act
            context.Principal = principal;
            context.HttpContext = httpContext;

            // Assert
            context.Principal.Should().BeSameAs(principal);
            context.HttpContext.Should().BeSameAs(httpContext);
        }
    }
}
