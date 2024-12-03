using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using static IdentityModel.OidcConstants;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class ResourceOwnerPasswordValidatorTests
    {
        private readonly Mock<UserManager<TestUser>> _userManager;
        private readonly Mock<SignInManager<TestUser>> _signInManager;
        private readonly Mock<ILogger<ResourceOwnerPasswordValidator<TestUser>>> _logger;
        private readonly ResourceOwnerPasswordValidator<TestUser> _validator;

        public ResourceOwnerPasswordValidatorTests()
        {
            var userStore = new Mock<IUserStore<TestUser>>();
            _userManager = new Mock<UserManager<TestUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<TestUser>>(_userManager.Object, null, null, null, null, null, null);
            _logger = new Mock<ILogger<ResourceOwnerPasswordValidator<TestUser>>>();
            
            _validator = new ResourceOwnerPasswordValidator<TestUser>(
                _userManager.Object,
                _signInManager.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task ValidateAsync_WhenUserExists_AndCredentialsValid_ShouldSucceed()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "password123"
            };

            var user = new TestUser { Id = "123", UserName = "testuser" };
            
            _userManager.Setup(x => x.FindByNameAsync(context.UserName))
                .ReturnsAsync(user);
            
            _userManager.Setup(x => x.GetUserIdAsync(user))
                .ReturnsAsync(user.Id);
                
            _signInManager.Setup(x => x.CheckPasswordSignInAsync(user, context.Password, true))
                .ReturnsAsync(SignInResult.Success);

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError == false);
            Assert.Equal(user.Id, context.Result.Subject);
            Assert.Equal(AuthenticationMethods.Password, context.Result.AuthenticationMethod);
        }

        [Fact]
        public async Task ValidateAsync_WhenUserDoesNotExist_ShouldReturnInvalidGrant()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "nonexistent",
                Password = "password123"
            };

            _userManager.Setup(x => x.FindByNameAsync(context.UserName))
                .ReturnsAsync((TestUser)null);

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError);
            Assert.Equal(TokenRequestErrors.InvalidGrant, context.Result.Error);
        }

        [Fact]
        public async Task ValidateAsync_WhenUserExists_ButPasswordInvalid_ShouldReturnInvalidGrant()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "wrongpassword"
            };

            var user = new TestUser { Id = "123", UserName = "testuser" };
            
            _userManager.Setup(x => x.FindByNameAsync(context.UserName))
                .ReturnsAsync(user);
                
            _signInManager.Setup(x => x.CheckPasswordSignInAsync(user, context.Password, true))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError);
            Assert.Equal(TokenRequestErrors.InvalidGrant, context.Result.Error);
        }
    }

    public class TestUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
