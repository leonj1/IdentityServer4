using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class IProfileServiceTests
    {
        private class TestProfileService : IProfileService
        {
            public bool IsActiveResult { get; set; } = true;
            public IEnumerable<Claim> ClaimsToReturn { get; set; } = new List<Claim>();

            public Task GetProfileDataAsync(ProfileDataRequestContext context)
            {
                context.IssuedClaims = new List<Claim>(ClaimsToReturn);
                return Task.CompletedTask;
            }

            public Task IsActiveAsync(IsActiveContext context)
            {
                context.IsActive = IsActiveResult;
                return Task.CompletedTask;
            }
        }

        private readonly TestProfileService _profileService;

        public IProfileServiceTests()
        {
            _profileService = new TestProfileService();
        }

        [Fact]
        public async Task GetProfileDataAsync_ShouldReturnConfiguredClaims()
        {
            // Arrange
            var expectedClaims = new List<Claim>
            {
                new Claim("name", "test"),
                new Claim("email", "test@test.com")
            };
            _profileService.ClaimsToReturn = expectedClaims;
            var context = new ProfileDataRequestContext();

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            context.IssuedClaims.Should().BeEquivalentTo(expectedClaims);
        }

        [Fact]
        public async Task IsActiveAsync_WhenActive_ShouldReturnTrue()
        {
            // Arrange
            _profileService.IsActiveResult = true;
            var context = new IsActiveContext(null, null, null);

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            context.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task IsActiveAsync_WhenInactive_ShouldReturnFalse()
        {
            // Arrange
            _profileService.IsActiveResult = false;
            var context = new IsActiveContext(null, null, null);

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            context.IsActive.Should().BeFalse();
        }
    }
}
