using System.Threading.Tasks;
using IdentityServer4.Validation;
using Xunit;
using System.Collections.Generic;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class CustomResponseResourceOwnerValidatorTests
    {
        private readonly CustomResponseResourceOwnerValidator _validator;

        public CustomResponseResourceOwnerValidatorTests()
        {
            _validator = new CustomResponseResourceOwnerValidator();
        }

        [Fact]
        public async Task ValidCredentials_ShouldReturnSuccessResult()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testUser",
                Password = "testUser" // Same as username for valid case
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.Equal("testUser", context.Result.Subject);
            Assert.Equal("password", context.Result.GrantType);
            Assert.NotNull(context.Result.CustomResponse);
            
            var response = context.Result.CustomResponse as Dictionary<string, object>;
            Assert.NotNull(response);
            Assert.Equal("some_string", response["string_value"]);
            Assert.Equal(42, response["int_value"]);
            Assert.NotNull(response["dto"]);
        }

        [Fact]
        public async Task InvalidCredentials_ShouldReturnFailureResult()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testUser",
                Password = "wrongPassword"
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.Equal(TokenRequestErrors.InvalidGrant, context.Result.Error);
            Assert.Equal("invalid_credential", context.Result.ErrorDescription);
            
            var response = context.Result.CustomResponse as Dictionary<string, object>;
            Assert.NotNull(response);
            Assert.Equal("some_string", response["string_value"]);
            Assert.Equal(42, response["int_value"]);
            Assert.NotNull(response["dto"]);
        }
    }
}
