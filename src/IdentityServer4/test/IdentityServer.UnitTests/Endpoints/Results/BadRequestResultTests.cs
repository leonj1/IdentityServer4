using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;
using System.Text.Json;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class BadRequestResultTests
    {
        [Fact]
        public async Task ExecuteAsync_WithErrorAndDescription_ShouldReturnCorrectResponse()
        {
            // Arrange
            var expectedError = "invalid_request";
            var expectedErrorDescription = "Invalid request parameter";
            var result = new BadRequestResult(expectedError, expectedErrorDescription);
            
            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            
            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            using var reader = new System.IO.StreamReader(context.Response.Body);
            var json = await reader.ReadToEndAsync();
            var response = JsonSerializer.Deserialize<BadRequestResult.ResultDto>(json);

            response.error.Should().Be(expectedError);
            response.error_description.Should().Be(expectedErrorDescription);
        }

        [Fact]
        public async Task ExecuteAsync_WithNoError_ShouldReturnOnlyStatusCode()
        {
            // Arrange
            var result = new BadRequestResult();
            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            
            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            using var reader = new System.IO.StreamReader(context.Response.Body);
            var content = await reader.ReadToEndAsync();
            content.Should().BeEmpty();
        }
    }
}
