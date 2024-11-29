using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Linq;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class CustomProfileServiceTests
    {
        private readonly TestUserStore _userStore;
        private readonly CustomProfileService _profileService;

        public CustomProfileServiceTests()
        {
            _userStore = new TestUserStore();
            var logger = new LoggerFactory().CreateLogger<TestUserProfileService>();
            _profileService = new CustomProfileService(_userStore, logger);
        }

        [Fact]
        public async Task When_Subject_Has_Custom_Authentication_And_Extra_Claim_Should_Include_Extra_Claim()
        {
            // Arrange
            var testUser = new TestUser
            {
                SubjectId = "123",
                Username = "test",
                Claims = new[] { new Claim("extra_claim", "extra_value") }
            };
            _userStore.Users.Add(testUser);

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("sub", "123"),
                    new Claim("extra_claim", "extra_value")
                }, "custom"))
            };

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            context.IssuedClaims.Should().Contain(c => 
                c.Type == "extra_claim" && c.Value == "extra_value");
        }

        [Fact]
        public async Task When_Subject_Has_Non_Custom_Authentication_Should_Not_Include_Extra_Claim()
        {
            // Arrange
            var testUser = new TestUser
            {
                SubjectId = "123",
                Username = "test",
                Claims = new[] { new Claim("extra_claim", "extra_value") }
            };
            _userStore.Users.Add(testUser);

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("sub", "123"),
                    new Claim("extra_claim", "extra_value")
                }, "different_auth"))
            };

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            context.IssuedClaims.Should().NotContain(c => 
                c.Type == "extra_claim" && c.Value == "extra_value");
        }
    }
}
