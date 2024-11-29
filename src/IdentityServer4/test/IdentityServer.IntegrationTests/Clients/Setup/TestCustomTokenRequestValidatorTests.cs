using System.Threading.Tasks;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class TestCustomTokenRequestValidatorTests
    {
        [Fact]
        public async Task ValidateAsync_ShouldAddCustomResponseToDictionary()
        {
            // Arrange
            var validator = new TestCustomTokenRequestValidator();
            var context = new CustomTokenRequestValidationContext
            {
                Result = new TokenRequestValidationResult(new ValidatedTokenRequest())
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result.CustomResponse);
            Assert.Contains("custom", context.Result.CustomResponse.Keys);
            Assert.Equal("custom", context.Result.CustomResponse["custom"]);
        }
    }
}
