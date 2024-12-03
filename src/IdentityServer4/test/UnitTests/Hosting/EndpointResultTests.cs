using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class EndpointResultTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var sut = new TestEndpointResult();

            // Act
            Func<Task> act = () => sut.ExecuteAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("context");
        }

        private class TestEndpointResult : IEndpointResult
        {
            public Task ExecuteAsync(HttpContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));
                    
                return Task.CompletedTask;
            }
        }
    }
}
