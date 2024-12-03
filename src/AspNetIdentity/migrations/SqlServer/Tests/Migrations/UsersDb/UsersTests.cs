using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;
using SqlServer.Migrations.UsersDb;

namespace SqlServer.Tests.Migrations.UsersDb
{
    public class UsersTests
    {
        [Fact]
        public void Up_Should_Create_Required_Tables()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<DbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            var options = builder.Options;
            var dbContext = new DbContext(options);
            var migrationBuilder = new MigrationBuilder(dbContext.Database.ProviderName);

            var migration = new Users();

            // Act
            migration.Up(migrationBuilder);
            var operations = migrationBuilder.Operations;

            // Assert
            Assert.Equal(7, operations.Count); // Verify we have 7 CreateTable operations
            
            // Verify essential tables are created
            Assert.Contains(operations, 
                op => op is CreateTableOperation cto && 
                      cto.Name == "AspNetUsers");
            
            Assert.Contains(operations, 
                op => op is CreateTableOperation cto && 
                      cto.Name == "AspNetRoles");
                      
            Assert.Contains(operations, 
                op => op is CreateTableOperation cto && 
                      cto.Name == "AspNetUserRoles");
        }

        [Fact]
        public void Down_Should_Drop_All_Tables()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<DbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            var options = builder.Options;
            var dbContext = new DbContext(options);
            var migrationBuilder = new MigrationBuilder(dbContext.Database.ProviderName);

            var migration = new Users();

            // Act
            migration.Down(migrationBuilder);
            var operations = migrationBuilder.Operations;

            // Assert
            Assert.Equal(7, operations.Count); // Verify we have 7 DropTable operations
            
            // Verify essential tables are dropped
            Assert.Contains(operations, 
                op => op is DropTableOperation dto && 
                      dto.Name == "AspNetUsers");
                      
            Assert.Contains(operations, 
                op => op is DropTableOperation dto && 
                      dto.Name == "AspNetRoles");
        }
    }
}
