using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Linq;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultProfileServiceTests
    {
        private readonly DefaultProfileService _subject;
        private readonly ILogger<DefaultProfileService> _logger = new LoggerFactory().CreateLogger<DefaultProfileService>();

        public DefaultProfileServiceTests()
        {
            _subject = new DefaultProfileService(_logger);
        }

        [Fact]
        public async Task GetProfileDataAsync_WhenCalled_ShouldIncludeRequestedClaims()
        {
            // Arrange
            var claims = new[]
            {
                new Claim("name", "Bob Smith"),
                new Claim("email", "bob@example.com")
            };
            var requestedClaimTypes = new[] { "name", "email" };
            
            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(claims)),
                RequestedClaimTypes = requestedClaimTypes
            };

            // Act
            await _subject.GetProfileDataAsync(context);

            // Assert
            context.IssuedClaims.Should().HaveCount(2);
            context.IssuedClaims.Should().Contain(x => x.Type == "name" && x.Value == "Bob Smith");
            context.IssuedClaims.Should().Contain(x => x.Type == "email" && x.Value == "bob@example.com");
        }

        [Fact]
        public async Task IsActiveAsync_WhenCalled_ShouldReturnTrue()
        {
            // Arrange
            var context = new IsActiveContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity()),
                Caller = "test"
            };

            // Act
            await _subject.IsActiveAsync(context);

            // Assert
            context.IsActive.Should().BeTrue();
        }
    }
}
