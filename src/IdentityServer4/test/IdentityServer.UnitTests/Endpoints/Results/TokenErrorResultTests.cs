using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Xunit;
using System.Text.Json;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class TokenErrorResultTests
    {
        private TokenErrorResult _subject;

        [Fact]
        public void Constructor_WithNullError_ThrowsArgumentNullException()
        {
            Action act = () => new TokenErrorResult(new TokenErrorResponse());
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("Error");
        }

        [Fact]
        public async Task ExecuteAsync_ContainsCorrectResponse()
        {
            // Arrange
            var response = new TokenErrorResponse
            {
                Error = "invalid_request",
                ErrorDescription = "error description",
                Custom = new Dictionary<string, object> { { "custom_field", "custom_value" } }
            };
            
            _subject = new TokenErrorResult(response);
            
            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await _subject.ExecuteAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            context.Response.Headers["Pragma"].ToString().Should().Contain("no-cache");

            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            var json = await new System.IO.StreamReader(context.Response.Body).ReadToEndAsync();
            var dto = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            dto["error"].ToString().Should().Be("invalid_request");
            dto["error_description"].ToString().Should().Be("error description");
            dto["custom_field"].ToString().Should().Be("custom_value");
        }
    }
}
