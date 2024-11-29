using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using IdentityServerHost.Data;

namespace IdentityServerHost.Tests
{
    public class SeedDataTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<ApplicationDbContext> _dbContext;

        public SeedDataTests()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            _dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(_ => _userManager.Object);
            serviceCollection.AddScoped(_ => _dbContext.Object);
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void EnsureSeedData_WhenAliceDoesNotExist_CreatesAliceUser()
        {
            // Arrange
            _userManager.Setup(x => x.FindByNameAsync("alice"))
                .ReturnsAsync((ApplicationUser)null);
            
            _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            _userManager.Setup(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim[]>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            SeedData.EnsureSeedData(_serviceProvider);

            // Assert
            _userManager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u => u.UserName == "alice"), 
                "Pass123$"), 
                Times.Once);
            
            _userManager.Verify(x => x.AddClaimsAsync(
                It.IsAny<ApplicationUser>(),
                It.Is<Claim[]>(claims => 
                    claims.Any(c => c.Type == JwtClaimTypes.Name && c.Value == "Alice Smith") &&
                    claims.Any(c => c.Type == JwtClaimTypes.Email && c.Value == "AliceSmith@email.com")
                )), 
                Times.Once);
        }

        [Fact]
        public void EnsureSeedData_WhenBobDoesNotExist_CreatesBobUser()
        {
            // Arrange
            _userManager.Setup(x => x.FindByNameAsync("bob"))
                .ReturnsAsync((ApplicationUser)null);
            
            _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            _userManager.Setup(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim[]>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            SeedData.EnsureSeedData(_serviceProvider);

            // Assert
            _userManager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u => u.UserName == "bob"), 
                "Pass123$"), 
                Times.Once);
            
            _userManager.Verify(x => x.AddClaimsAsync(
                It.IsAny<ApplicationUser>(),
                It.Is<Claim[]>(claims => 
                    claims.Any(c => c.Type == JwtClaimTypes.Name && c.Value == "Bob Smith") &&
                    claims.Any(c => c.Type == JwtClaimTypes.Email && c.Value == "BobSmith@email.com") &&
                    claims.Any(c => c.Type == "location" && c.Value == "somewhere")
                )), 
                Times.Once);
        }

        [Fact]
        public void EnsureSeedData_WhenUsersExist_DoesNotCreateUsers()
        {
            // Arrange
            var alice = new ApplicationUser { UserName = "alice" };
            var bob = new ApplicationUser { UserName = "bob" };

            _userManager.Setup(x => x.FindByNameAsync("alice"))
                .ReturnsAsync(alice);
            _userManager.Setup(x => x.FindByNameAsync("bob"))
                .ReturnsAsync(bob);

            // Act
            SeedData.EnsureSeedData(_serviceProvider);

            // Assert
            _userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            _userManager.Verify(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim[]>()), Times.Never);
        }
    }
}
