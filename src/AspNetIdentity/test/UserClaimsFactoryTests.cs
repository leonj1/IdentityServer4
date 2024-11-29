using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Moq;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class UserClaimsFactoryTests
    {
        private readonly Mock<IUserClaimsPrincipalFactory<TestUser>> _innerFactoryMock;
        private readonly Mock<UserManager<TestUser>> _userManagerMock;
        private readonly UserClaimsFactory<TestUser> _subject;
        private readonly TestUser _user;

        public UserClaimsFactoryTests()
        {
            _user = new TestUser { Id = "123", UserName = "test" };
            
            _innerFactoryMock = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            var decorator = new Decorator<IUserClaimsPrincipalFactory<TestUser>>(_innerFactoryMock.Object);
            
            var userStore = new Mock<IUserStore<TestUser>>();
            _userManagerMock = new Mock<UserManager<TestUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            
            _subject = new UserClaimsFactory<TestUser>(decorator, _userManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenSubjectClaimMissing_ShouldAddSubjectClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            
            _innerFactoryMock.Setup(x => x.CreateAsync(_user))
                .ReturnsAsync(principal);
            
            _userManagerMock.Setup(x => x.GetUserIdAsync(_user))
                .ReturnsAsync("123");
            
            _userManagerMock.Setup(x => x.GetUserNameAsync(_user))
                .ReturnsAsync("test");

            // Act
            var result = await _subject.CreateAsync(_user);

            // Assert
            var subClaim = result.FindFirst(JwtClaimTypes.Subject);
            Assert.NotNull(subClaim);
            Assert.Equal("123", subClaim.Value);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailSupported_ShouldAddEmailClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            
            _innerFactoryMock.Setup(x => x.CreateAsync(_user))
                .ReturnsAsync(principal);
            
            _userManagerMock.Setup(x => x.GetUserIdAsync(_user))
                .ReturnsAsync("123");
            
            _userManagerMock.Setup(x => x.GetUserNameAsync(_user))
                .ReturnsAsync("test");
                
            _userManagerMock.Setup(x => x.SupportsUserEmail)
                .Returns(true);
                
            _userManagerMock.Setup(x => x.GetEmailAsync(_user))
                .ReturnsAsync("test@test.com");
                
            _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(_user))
                .ReturnsAsync(true);

            // Act
            var result = await _subject.CreateAsync(_user);

            // Assert
            var emailClaim = result.FindFirst(JwtClaimTypes.Email);
            Assert.NotNull(emailClaim);
            Assert.Equal("test@test.com", emailClaim.Value);
            
            var emailVerifiedClaim = result.FindFirst(JwtClaimTypes.EmailVerified);
            Assert.NotNull(emailVerifiedClaim);
            Assert.Equal("true", emailVerifiedClaim.Value);
        }

        [Fact]
        public async Task CreateAsync_WhenPhoneNumberSupported_ShouldAddPhoneClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            
            _innerFactoryMock.Setup(x => x.CreateAsync(_user))
                .ReturnsAsync(principal);
            
            _userManagerMock.Setup(x => x.GetUserIdAsync(_user))
                .ReturnsAsync("123");
            
            _userManagerMock.Setup(x => x.GetUserNameAsync(_user))
                .ReturnsAsync("test");
                
            _userManagerMock.Setup(x => x.SupportsUserPhoneNumber)
                .Returns(true);
                
            _userManagerMock.Setup(x => x.GetPhoneNumberAsync(_user))
                .ReturnsAsync("1234567890");
                
            _userManagerMock.Setup(x => x.IsPhoneNumberConfirmedAsync(_user))
                .ReturnsAsync(true);

            // Act
            var result = await _subject.CreateAsync(_user);

            // Assert
            var phoneClaim = result.FindFirst(JwtClaimTypes.PhoneNumber);
            Assert.NotNull(phoneClaim);
            Assert.Equal("1234567890", phoneClaim.Value);
            
            var phoneVerifiedClaim = result.FindFirst(JwtClaimTypes.PhoneNumberVerified);
            Assert.NotNull(phoneVerifiedClaim);
            Assert.Equal("true", phoneVerifiedClaim.Value);
        }
    }

    public class TestUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
