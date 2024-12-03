using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class TokenRevocationErrorResultTests
    {
        [Fact]
        public void Constructor_ShouldSetErrorProperty()
        {
            // Arrange
            var expectedError = "invalid_request";

            // Act
            var result = new TokenRevocationErrorResult(expectedError);

            // Assert
            result.Error.Should().Be(expectedError);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetCorrectStatusCodeAndReturnJson()
        {
            // Arrange
            var expectedError = "invalid_request";
            var result = new TokenRevocationErrorResult(expectedError);
            
            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            context.Response.ContentType.Should().StartWith("application/json");
            
            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            var json = await new System.IO.StreamReader(context.Response.Body).ReadToEndAsync();
            json.Should().Be("{\"error\":\"invalid_request\"}");
        }
    }
}
