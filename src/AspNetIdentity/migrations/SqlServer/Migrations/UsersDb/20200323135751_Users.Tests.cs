using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using IdentityServerHost.Data;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;

namespace SqlServer.Migrations.UsersDb.Tests
{
    public class UsersMigrationTests
    {
        private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void Users_Migration_Creates_Expected_Schema()
        {
            // Arrange
            var options = GetDbContextOptions();

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                // Assert
                Assert.True(context.Model.FindEntityType(typeof(ApplicationUser)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityRole)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityUserClaim<string>)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityUserRole<string>)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityUserLogin<string>)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityUserToken<string>)) != null);
                Assert.True(context.Model.FindEntityType(typeof(IdentityRoleClaim<string>)) != null);
            }
        }

        [Fact]
        public void ApplicationUser_Has_Expected_Properties()
        {
            // Arrange
            var options = GetDbContextOptions();

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                var userType = context.Model.FindEntityType(typeof(ApplicationUser));

                // Assert
                Assert.NotNull(userType);
                Assert.NotNull(userType.FindProperty("Id"));
                Assert.NotNull(userType.FindProperty("UserName"));
                Assert.NotNull(userType.FindProperty("Email"));
                Assert.NotNull(userType.FindProperty("PhoneNumber"));
                Assert.NotNull(userType.FindProperty("LockoutEnabled"));
                Assert.NotNull(userType.FindProperty("TwoFactorEnabled"));
            }
        }

        [Fact]
        public void IdentityRole_Has_Expected_Properties()
        {
            // Arrange
            var options = GetDbContextOptions();

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                var roleType = context.Model.FindEntityType(typeof(IdentityRole));

                // Assert
                Assert.NotNull(roleType);
                Assert.NotNull(roleType.FindProperty("Id"));
                Assert.NotNull(roleType.FindProperty("Name"));
                Assert.NotNull(roleType.FindProperty("NormalizedName"));
                Assert.NotNull(roleType.FindProperty("ConcurrencyStamp"));
            }
        }
    }
}
