using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace IdentityServer4.UnitTests
{
    public class TestUserProfileServiceTests
    {
        private readonly Mock<ILogger<TestUserProfileService>> _logger;
        private readonly TestUserStore _userStore;
        private readonly TestUserProfileService _profileService;
        private readonly TestUser _testUser;

        public TestUserProfileServiceTests()
        {
            _logger = new Mock<ILogger<TestUserProfileService>>();
            
            // Create test user
            _testUser = new TestUser
            {
                SubjectId = "123",
                Username = "test",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("name", "Test User"),
                    new Claim("email", "test@test.com")
                }
            };

            _userStore = new TestUserStore(new List<TestUser> { _testUser });
            _profileService = new TestUserProfileService(_userStore, _logger.Object);
        }

        [Fact]
        public async Task GetProfileData_WhenUserExists_ShouldReturnRequestedClaims()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            var context = new ProfileDataRequestContext(subject, new Client(), "caller", 
                new[] { "name", "email" });

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            Assert.NotEmpty(context.IssuedClaims);
            Assert.Equal(2, context.IssuedClaims.Count());
            Assert.Contains(context.IssuedClaims, c => c.Type == "name");
            Assert.Contains(context.IssuedClaims, c => c.Type == "email");
        }

        [Fact]
        public async Task GetProfileData_WhenUserDoesNotExist_ShouldReturnNoClaims()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "456") // Non-existent user
            }));

            var context = new ProfileDataRequestContext(subject, new Client(), "caller",
                new[] { "name", "email" });

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            Assert.Empty(context.IssuedClaims);
        }

        [Fact]
        public async Task IsActive_WhenUserExistsAndActive_ShouldReturnTrue()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            var context = new IsActiveContext(subject, new Client(), "caller");

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            Assert.True(context.IsActive);
        }

        [Fact]
        public async Task IsActive_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "456") // Non-existent user
            }));

            var context = new IsActiveContext(subject, new Client(), "caller");

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            Assert.False(context.IsActive);
        }

        [Fact]
        public async Task IsActive_WhenUserExistsButInactive_ShouldReturnFalse()
        {
            // Arrange
            _testUser.IsActive = false;
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            var context = new IsActiveContext(subject, new Client(), "caller");

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            Assert.False(context.IsActive);
        }
    }
}
