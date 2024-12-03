using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServerHost.Extensions.Tests
{
    public class HostProfileServiceTests
    {
        private readonly TestUserStore _userStore;
        private readonly Mock<ILogger<TestUserProfileService>> _logger;
        private readonly HostProfileService _profileService;

        public HostProfileServiceTests()
        {
            _userStore = new TestUserStore(new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "123",
                    Username = "test",
                    Claims = new List<Claim> { new Claim("name", "Test User") }
                }
            });
            _logger = new Mock<ILogger<TestUserProfileService>>();
            _profileService = new HostProfileService(_userStore, _logger.Object);
        }

        [Fact]
        public async Task GetProfileData_WithoutTransaction_ShouldNotAddTransactionClaim()
        {
            // Arrange
            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("sub", "123") })),
                RequestedResources = new ResourceValidationResult
                {
                    ParsedScopes = new[] { new ParsedScopeValue("openid") }
                },
                IssuedClaims = new List<Claim>()
            };

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            Assert.DoesNotContain(context.IssuedClaims, c => c.Type == "transaction_id");
        }

        [Fact]
        public async Task GetProfileData_WithTransaction_ShouldAddTransactionClaim()
        {
            // Arrange
            var transactionId = "tx123";
            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("sub", "123") })),
                RequestedResources = new ResourceValidationResult
                {
                    ParsedScopes = new[] { new ParsedScopeValue("transaction", transactionId) }
                },
                IssuedClaims = new List<Claim>()
            };

            // Act
            await _profileService.GetProfileDataAsync(context);

            // Assert
            var transactionClaim = context.IssuedClaims.FirstOrDefault(c => c.Type == "transaction_id");
            Assert.NotNull(transactionClaim);
            Assert.Equal(transactionId, transactionClaim.Value);
        }
    }
}
