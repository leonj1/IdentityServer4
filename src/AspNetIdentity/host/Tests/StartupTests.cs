using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using IdentityServerHost.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerHost.Tests
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_RegistersRequiredServices()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x.GetConnectionString("DefaultConnection"))
                .Returns("TestConnectionString");
            
            var startup = new Startup(configuration.Object);
            var services = new ServiceCollection();

            // Act
            startup.ConfigureServices(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            
            // Verify essential services are registered
            Assert.NotNull(serviceProvider.GetService<DbContext>());
            Assert.NotNull(serviceProvider.GetService<UserManager<ApplicationUser>>());
            Assert.NotNull(serviceProvider.GetService<SignInManager<ApplicationUser>>());
            Assert.NotNull(serviceProvider.GetService<IIdentityServerBuilder>());
        }

        [Fact]
        public void Configure_RegistersMiddleware()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var startup = new Startup(configuration.Object);
            var appBuilder = new Mock<IApplicationBuilder>();
            var env = new Mock<IWebHostEnvironment>();
            
            env.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            startup.Configure(appBuilder.Object, env.Object);

            // Assert
            appBuilder.Verify(x => x.UseRouting(), Times.Once);
            appBuilder.Verify(x => x.UseIdentityServer(), Times.Once);
            appBuilder.Verify(x => x.UseAuthorization(), Times.Once);
            appBuilder.Verify(x => x.UseStaticFiles(), Times.Once);
        }
    }
}
