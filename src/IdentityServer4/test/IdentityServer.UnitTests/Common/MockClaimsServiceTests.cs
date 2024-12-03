using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using IdentityServer4.Validation;

namespace IdentityServer.UnitTests.Common
{
    public class MockClaimsServiceTests
    {
        [Fact]
        public async Task GetIdentityTokenClaims_ShouldReturnConfiguredClaims()
        {
            // Arrange
            var mockService = new MockClaimsService();
            var expectedClaim = new Claim("test_type", "test_value");
            mockService.IdentityTokenClaims.Add(expectedClaim);

            // Act
            var result = await mockService.GetIdentityTokenClaimsAsync(
                new ClaimsPrincipal(), 
                new ResourceValidationResult(), 
                false, 
                new ValidatedRequest());

            // Assert
            var claims = result.ToList();
            Assert.Single(claims);
            Assert.Equal(expectedClaim.Type, claims[0].Type);
            Assert.Equal(expectedClaim.Value, claims[0].Value);
        }

        [Fact]
        public async Task GetAccessTokenClaims_ShouldReturnConfiguredClaims()
        {
            // Arrange
            var mockService = new MockClaimsService();
            var expectedClaim = new Claim("access_type", "access_value");
            mockService.AccessTokenClaims.Add(expectedClaim);

            // Act
            var result = await mockService.GetAccessTokenClaimsAsync(
                new ClaimsPrincipal(),
                new ResourceValidationResult(),
                new ValidatedRequest());

            // Assert
            var claims = result.ToList();
            Assert.Single(claims);
            Assert.Equal(expectedClaim.Type, claims[0].Type);
            Assert.Equal(expectedClaim.Value, claims[0].Value);
        }

        [Fact]
        public async Task GetIdentityTokenClaims_ShouldReturnEmptyWhenNoClaimsConfigured()
        {
            // Arrange
            var mockService = new MockClaimsService();

            // Act
            var result = await mockService.GetIdentityTokenClaimsAsync(
                new ClaimsPrincipal(),
                new ResourceValidationResult(),
                false,
                new ValidatedRequest());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAccessTokenClaims_ShouldReturnEmptyWhenNoClaimsConfigured()
        {
            // Arrange
            var mockService = new MockClaimsService();

            // Act
            var result = await mockService.GetAccessTokenClaimsAsync(
                new ClaimsPrincipal(),
                new ResourceValidationResult(),
                new ValidatedRequest());

            // Assert
            Assert.Empty(result);
        }
    }
}
