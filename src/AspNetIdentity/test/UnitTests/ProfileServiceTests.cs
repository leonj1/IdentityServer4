using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class ProfileServiceTests
    {
        private readonly Mock<UserManager<TestUser>> _userManager;
        private readonly Mock<IUserClaimsPrincipalFactory<TestUser>> _claimsFactory;
        private readonly Mock<ILogger<ProfileService<TestUser>>> _logger;
        private readonly ProfileService<TestUser> _profileService;
        
        public ProfileServiceTests()
        {
            var userStore = new Mock<IUserStore<TestUser>>();
            _userManager = new Mock<UserManager<TestUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _claimsFactory = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            _logger = new Mock<ILogger<ProfileService<TestUser>>>();
            
            _profileService = new ProfileService<TestUser>(
                _userManager.Object,
                _claimsFactory.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task GetProfileDataAsync_WhenSubjectIdIsNull_ThrowsException()
        {
            // Arrange
            var context = new ProfileDataRequestContext(
                new ClaimsPrincipal(),
                new Client(),
                "caller",
                new string[] { }
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _profileService.GetProfileDataAsync(context));
        }

        [Fact]
        public async Task GetProfileDataAsync_WhenUserNotFound_DoesNotAddClaims()
        {
            // Arrange
            var subjectId = "123";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", subjectId)
            }));

            var context = new ProfileDataRequestContext(
                principal,
                new Client(),
                "caller",
                new string[] { }
            );

            _userManager.Setup(x => x.FindByIdAsync(subjectId))
                .ReturnsAsync((TestUser)null);

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            Assert.Empty(context.IssuedClaims);
        }

        [Fact]
        public async Task GetProfileDataAsync_WhenUserFound_AddsClaims()
        {
            // Arrange
            var subjectId = "123";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", subjectId)
            }));

            var context = new ProfileDataRequestContext(
                principal,
                new Client(),
                "caller",
                new string[] { }
            );

            var user = new TestUser { Id = subjectId };
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("name", "Test User"),
                new Claim("email", "test@test.com")
            }));

            _userManager.Setup(x => x.FindByIdAsync(subjectId))
                .ReturnsAsync(user);
            _claimsFactory.Setup(x => x.CreateAsync(user))
                .ReturnsAsync(userClaims);

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            Assert.Equal(2, context.IssuedClaims.Count);
        }

        [Fact]
        public async Task IsActiveAsync_WhenSubjectIdIsNull_ThrowsException()
        {
            // Arrange
            var context = new IsActiveContext(
                new ClaimsPrincipal(),
                new Client(),
                "caller"
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _profileService.IsActiveAsync(context));
        }

        [Fact]
        public async Task IsActiveAsync_WhenUserNotFound_SetsIsActiveToFalse()
        {
            // Arrange
            var subjectId = "123";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", subjectId)
            }));

            var context = new IsActiveContext(
                principal,
                new Client(),
                "caller"
            );

            _userManager.Setup(x => x.FindByIdAsync(subjectId))
                .ReturnsAsync((TestUser)null);

            // Act
            await _profileService.IsActiveAsync(context);

            // Assert
            Assert.False(context.IsActive);
        }

        private class TestUser
        {
            public string Id { get; set; }
        }
    }
}
