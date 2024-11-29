using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class ProtectedResourceErrorResultTests
    {
        [Fact]
        public async Task Simple_Error_Should_Return_401()
        {
            // Arrange
            var result = new ProtectedResourceErrorResult("error");
            var ctx = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(ctx);

            // Assert
            ctx.Response.StatusCode.Should().Be(401);
            ctx.Response.Headers[HeaderNames.WWWAuthenticate].ToString()
                .Should().Contain("Bearer realm=\"IdentityServer\"")
                .And.Contain("error=\"error\"");
        }

        [Fact]
        public async Task Error_With_Description_Should_Return_Complete_Header()
        {
            // Arrange
            var result = new ProtectedResourceErrorResult("error", "description");
            var ctx = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(ctx);

            // Assert
            ctx.Response.StatusCode.Should().Be(401);
            ctx.Response.Headers[HeaderNames.WWWAuthenticate].ToString()
                .Should().Contain("Bearer realm=\"IdentityServer\"")
                .And.Contain("error=\"error\"")
                .And.Contain("error_description=\"description\"");
        }

        [Fact]
        public async Task ExpiredToken_Should_Be_Converted_To_InvalidToken()
        {
            // Arrange
            var result = new ProtectedResourceErrorResult(
                OidcConstants.ProtectedResourceErrors.ExpiredToken);
            var ctx = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(ctx);

            // Assert
            ctx.Response.StatusCode.Should().Be(401);
            ctx.Response.Headers[HeaderNames.WWWAuthenticate].ToString()
                .Should().Contain("Bearer realm=\"IdentityServer\"")
                .And.Contain($"error=\"{OidcConstants.ProtectedResourceErrors.InvalidToken}\"")
                .And.Contain("error_description=\"The access token expired\"");
        }

        [Fact]
        public async Task Cache_Headers_Should_Be_Set()
        {
            // Arrange
            var result = new ProtectedResourceErrorResult("error");
            var ctx = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(ctx);

            // Assert
            ctx.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            ctx.Response.Headers["Pragma"].ToString().Should().Contain("no-cache");
        }
    }
}
