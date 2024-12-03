using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class StatusCodeResultTests
    {
        [Fact]
        public void Constructor_with_HttpStatusCode_should_set_status_code()
        {
            var result = new StatusCodeResult(HttpStatusCode.NotFound);
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void Constructor_with_int_should_set_status_code()
        {
            var result = new StatusCodeResult(402);
            result.StatusCode.Should().Be(402);
        }

        [Fact]
        public async Task ExecuteAsync_should_set_response_status_code()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var result = new StatusCodeResult(HttpStatusCode.NotFound);

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(404);
        }
    }
}
