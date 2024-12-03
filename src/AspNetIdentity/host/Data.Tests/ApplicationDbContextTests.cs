using Microsoft.EntityFrameworkCore;
using Xunit;
using IdentityServerHost.Data;

namespace IdentityServerHost.Data.Tests
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void OnModelCreating_ShouldNotThrowException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            // Act & Assert
            using (var context = new ApplicationDbContext(options))
            {
                Assert.NotNull(context);
                // Verify the context can be created without throwing exceptions
                context.Database.EnsureCreated();
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb2")
                .Options;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                // Assert
                Assert.NotNull(context);
                Assert.IsType<ApplicationDbContext>(context);
            }
        }
    }
}
