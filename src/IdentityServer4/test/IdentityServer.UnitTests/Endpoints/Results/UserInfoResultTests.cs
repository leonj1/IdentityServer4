using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;
using System.Text.Json;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class UserInfoResultTests
    {
        [Fact]
        public async Task UserInfoResult_Should_Contain_Claims()
        {
            // Arrange
            var claims = new Dictionary<string, object>
            {
                { "sub", "123" },
                { "name", "Test User" }
            };
            
            var result = new UserInfoResult(claims);
            
            // Act
            var context = new DefaultHttpContext();
            await result.ExecuteAsync(context);

            // Assert
            context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            context.Response.Headers["Pragma"].ToString().Should().Contain("no-cache");
            
            var responseJson = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
                context.Response.Body, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            
            responseJson.Should().BeEquivalentTo(claims);
        }
    }
}
