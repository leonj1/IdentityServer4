using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.IntegrationTests.Extensibility
{
    public class CustomProfileServiceUnitTests
    {
        private readonly CustomProfileService _profileService;

        public CustomProfileServiceUnitTests()
        {
            _profileService = new CustomProfileService();
        }

        [Fact]
        public async Task GetProfileDataAsync_ShouldAddFooClaim()
        {
            // Arrange
            var context = new ProfileDataRequestContext();

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            context.IssuedClaims.Should().HaveCount(1);
            var claim = context.IssuedClaims.First();
            claim.Type.Should().Be("foo");
            claim.Value.Should().Be("bar");
        }

        [Fact]
        public async Task IsActiveAsync_ShouldReturnTrue()
        {
            // Arrange
            var context = new IsActiveContext("subject", null, null);

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            context.IsActive.Should().BeTrue();
        }
    }
}
