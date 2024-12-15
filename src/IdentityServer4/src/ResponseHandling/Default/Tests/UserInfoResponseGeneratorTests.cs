using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.ResponseHandling.Tests
{
    public class UserInfoResponseGeneratorTests
    {
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly Mock<IResourceStore> _mockResourceStore;
        private readonly Mock<ILogger<UserInfoResponseGenerator>> _mockLogger;
        private readonly UserInfoResponseGenerator _generator;

        public UserInfoResponseGeneratorTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _mockResourceStore = new Mock<IResourceStore>();
            _mockLogger = new Mock<ILogger<UserInfoResponseGenerator>>();
            _generator = new UserInfoResponseGenerator(
                _mockProfileService.Object,
                _mockResourceStore.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ProcessAsync_WhenValidRequest_ReturnsExpectedClaims()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
            { 
                new Claim("sub", "123") 
            }));
            
            var validationResult = new UserInfoRequestValidationResult
            {
                Subject = subject,
                TokenValidationResult = new TokenValidationResult
                {
                    Claims = new List<Claim> 
                    { 
                        new Claim("scope", "openid")
                    },
                    Client = new Client()
                }
            };

            _mockResourceStore.Setup(rs => rs.FindEnabledScopesAsync(It.IsAny<IEnumerable<string>>()))
                              .ReturnsAsync(new List<string> { "openid" });

            _mockProfileService
                .Setup(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()))
                .Callback<ProfileDataRequestContext>(context =>
                {
                    context.IssuedClaims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "John Doe"),
                        new Claim(JwtClaimTypes.Email, "john.doe@example.com")
                    };
                });

            // Act
            var result = await _generator.ProcessAsync(validationResult);

            // Assert
            Assert.Contains(new Claim(JwtClaimTypes.Name, "John Doe"), result);
            Assert.Contains(new Claim(JwtClaimTypes.Email, "john.doe@example.com"), result);
        }

        [Fact]
        public async Task ProcessAsync_WhenIncorrectSubject_ThrowsException()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
            { 
                new Claim("sub", "123") 
            }));
            
            var validationResult = new UserInfoRequestValidationResult
            {
                Subject = subject,
                TokenValidationResult = new TokenValidationResult
                {
                    Claims = new List<Claim>(),
                    Client = new Client()
                }
            };

            _mockProfileService
                .Setup(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()))
                .Callback<ProfileDataRequestContext>(context =>
                {
                    context.IssuedClaims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Subject, "wrong-subject")
                    };
                });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _generator.ProcessAsync(validationResult)
            );
        }

        [Fact]
        public async Task ProcessAsync_WhenNoClaims_ReturnsEmptyList()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
            { 
                new Claim("sub", "123") 
            }));
            
            var validationResult = new UserInfoRequestValidationResult
            {
                Subject = subject,
                TokenValidationResult = new TokenValidationResult
                {
                    Claims = new List<Claim>(),
                    Client = new Client()
                }
            };

            _mockProfileService
                .Setup(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()))
                .Callback<ProfileDataRequestContext>(context =>
                {
                    context.IssuedClaims = new List<Claim>();
                });

            // Act
            var result = await _generator.ProcessAsync(validationResult);

            // Assert
            Assert.Empty(result);
        }
    }
}
