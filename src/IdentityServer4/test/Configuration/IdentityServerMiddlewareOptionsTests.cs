using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class IdentityServerMiddlewareOptionsTests
    {
        [Fact]
        public void DefaultAuthenticationMiddleware_ShouldNotBeNull()
        {
            // Arrange
            var options = new IdentityServerMiddlewareOptions();

            // Assert
            Assert.NotNull(options.AuthenticationMiddleware);
        }

        [Fact]
        public void CustomAuthenticationMiddleware_ShouldBeAssignable()
        {
            // Arrange
            var options = new IdentityServerMiddlewareOptions();
            Action<IApplicationBuilder> customMiddleware = (app) => { };

            // Act
            options.AuthenticationMiddleware = customMiddleware;

            // Assert
            Assert.Equal(customMiddleware, options.AuthenticationMiddleware);
        }
    }
}
