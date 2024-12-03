using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Logging.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Logging
{
    public class EndSessionRequestValidationLogTests
    {
        [Fact]
        public void Should_Serialize_Full_Validation_Request()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "client",
                ClientName = "Test Client"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(JwtClaimTypes.Subject, "123")
            }));

            var request = new ValidatedEndSessionRequest
            {
                Client = client,
                Subject = user,
                Raw = new System.Collections.Specialized.NameValueCollection
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                },
                PostLogOutUri = "https://client/signout-callback",
                State = "abc123"
            };

            // Act
            var log = new EndSessionRequestValidationLog(request);
            var json = log.ToString();

            // Assert
            log.ClientId.Should().Be("client");
            log.ClientName.Should().Be("Test Client");
            log.SubjectId.Should().Be("123");
            log.PostLogOutUri.Should().Be("https://client/signout-callback");
            log.State.Should().Be("abc123");
            log.Raw.Should().ContainKeys("key1", "key2");
            log.Raw["key1"].Should().Be("value1");
            log.Raw["key2"].Should().Be("value2");
        }

        [Fact]
        public void Should_Handle_Missing_Values()
        {
            // Arrange
            var request = new ValidatedEndSessionRequest
            {
                Raw = new System.Collections.Specialized.NameValueCollection()
            };

            // Act
            var log = new EndSessionRequestValidationLog(request);

            // Assert
            log.ClientId.Should().BeNull();
            log.ClientName.Should().BeNull();
            log.SubjectId.Should().Be("unknown");
            log.PostLogOutUri.Should().BeNull();
            log.State.Should().BeNull();
            log.Raw.Should().BeEmpty();
        }
    }
}
