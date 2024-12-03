using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ValidatedEndSessionRequestTests
    {
        [Fact]
        public void IsAuthenticated_WithNoClient_ReturnsFalse()
        {
            var request = new ValidatedEndSessionRequest();
            request.IsAuthenticated.Should().BeFalse();
        }

        [Fact]
        public void IsAuthenticated_WithClient_ReturnsTrue()
        {
            var request = new ValidatedEndSessionRequest
            {
                Client = new Client()
            };
            request.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            var request = new ValidatedEndSessionRequest
            {
                PostLogOutUri = "https://example.com/logout",
                State = "abc123",
                ClientIds = new[] { "client1", "client2" }
            };

            request.PostLogOutUri.Should().Be("https://example.com/logout");
            request.State.Should().Be("abc123");
            request.ClientIds.Should().BeEquivalentTo(new[] { "client1", "client2" });
        }
    }
}
