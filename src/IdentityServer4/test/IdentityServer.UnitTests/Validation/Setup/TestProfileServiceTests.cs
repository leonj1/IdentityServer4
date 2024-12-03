using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestProfileServiceTests
    {
        [Fact]
        public async Task GetProfileDataAsync_Should_Complete_Successfully()
        {
            // Arrange
            var service = new TestProfileService();
            var context = new ProfileDataRequestContext();

            // Act
            await service.GetProfileDataAsync(context);

            // Assert
            // Method should complete without throwing exceptions
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsActiveAsync_Should_Set_Expected_IsActive_Value(bool shouldBeActive)
        {
            // Arrange
            var service = new TestProfileService(shouldBeActive);
            var context = new IsActiveContext(null, null, null);

            // Act
            await service.IsActiveAsync(context);

            // Assert
            context.IsActive.Should().Be(shouldBeActive);
        }
    }
}
