using IdentityServer4.Models.ManageViewModels;
using Xunit;

namespace IdentityServer4.UnitTests.Models.ManageViewModels
{
    public class GenerateRecoveryCodesViewModelTests
    {
        [Fact]
        public void RecoveryCodes_ShouldBeSettableAndGettable()
        {
            // Arrange
            var expectedCodes = new[] { "code1", "code2", "code3" };
            var viewModel = new GenerateRecoveryCodesViewModel();

            // Act
            viewModel.RecoveryCodes = expectedCodes;

            // Assert
            Assert.NotNull(viewModel.RecoveryCodes);
            Assert.Equal(expectedCodes, viewModel.RecoveryCodes);
            Assert.Equal(3, viewModel.RecoveryCodes.Length);
        }

        [Fact]
        public void RecoveryCodes_ShouldAllowNull()
        {
            // Arrange
            var viewModel = new GenerateRecoveryCodesViewModel();

            // Act & Assert
            Assert.Null(viewModel.RecoveryCodes);
        }
    }
}
