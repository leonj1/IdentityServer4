using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class IntrospectionResultTests
    {
        [Fact]
        public void Constructor_WithNullEntries_ThrowsArgumentNullException()
        {
            Action act = () => new IntrospectionResult(null);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("entries");
        }

        [Fact]
        public async Task ExecuteAsync_SetsNoCacheHeaders()
        {
            // Arrange
            var entries = new Dictionary<string, object>
            {
                { "active", true },
                { "sub", "123" }
            };
            var result = new IntrospectionResult(entries);
            var context = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            context.Response.Headers["Pragma"].ToString().Should().Contain("no-cache");
        }

        [Fact]
        public async Task ExecuteAsync_WritesEntriesAsJson()
        {
            // Arrange
            var entries = new Dictionary<string, object>
            {
                { "active", true },
                { "sub", "123" }
            };
            var result = new IntrospectionResult(entries);
            var context = new DefaultHttpContext();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.ContentType.Should().StartWith("application/json");
        }
    }
}
