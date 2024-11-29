using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServerHost.Data;
using Xunit;
using Moq;
using System.Linq;

namespace SqlServer.Tests
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_RegistersRequiredServices()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var connectionString = "TestConnectionString";
            configuration.Setup(c => c.GetConnectionString("db")).Returns(connectionString);
            
            var startup = new Startup(configuration.Object);
            var services = new ServiceCollection();

            // Act
            startup.ConfigureServices(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify DbContext is registered
            var dbContext = services.FirstOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            Assert.NotNull(dbContext);

            // Verify Identity services are registered
            var identityBuilder = services.FirstOrDefault(d => 
                d.ServiceType == typeof(UserManager<ApplicationUser>));
            Assert.NotNull(identityBuilder);

            var roleManager = services.FirstOrDefault(d => 
                d.ServiceType == typeof(RoleManager<IdentityRole>));
            Assert.NotNull(roleManager);

            // Verify token providers are registered
            var tokenProvider = services.FirstOrDefault(d => 
                d.ServiceType == typeof(IUserTwoFactorTokenProvider<ApplicationUser>));
            Assert.NotNull(tokenProvider);
        }
    }
}
