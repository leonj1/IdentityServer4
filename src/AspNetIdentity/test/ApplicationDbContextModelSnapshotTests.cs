using System;
using System.Linq;
using IdentityServerHost.Data;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace IdentityServer4.AspNetIdentity.Test
{
    public class ApplicationDbContextModelSnapshotTests
    {
        private readonly ModelBuilder _modelBuilder;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextModelSnapshotTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _modelBuilder = new ModelBuilder();
            
            // Build the model as defined in the snapshot
            new ApplicationDbContextModelSnapshot().BuildModel(_modelBuilder);
        }

        [Fact]
        public void ApplicationUser_HasCorrectProperties()
        {
            var entityType = _modelBuilder.Model.FindEntityType(typeof(ApplicationUser));
            Assert.NotNull(entityType);

            // Verify key properties
            var keys = entityType.GetKeys().ToList();
            Assert.Single(keys);
            Assert.Equal("Id", keys[0].Properties[0].Name);

            // Verify indexes
            var indexes = entityType.GetIndexes().ToList();
            Assert.Equal(2, indexes.Count);
            Assert.Contains(indexes, i => i.Properties[0].Name == "NormalizedEmail");
            Assert.Contains(indexes, i => i.Properties[0].Name == "NormalizedUserName");
        }

        [Fact]
        public void IdentityRole_HasCorrectConfiguration()
        {
            var entityType = _modelBuilder.Model.FindEntityType(typeof(IdentityRole));
            Assert.NotNull(entityType);

            // Verify table name
            Assert.Equal("AspNetRoles", entityType.GetTableName());

            // Verify properties
            Assert.True(entityType.FindProperty("Id") != null);
            Assert.True(entityType.FindProperty("Name") != null);
            Assert.True(entityType.FindProperty("NormalizedName") != null);
        }

        [Fact]
        public void UserClaims_HasCorrectRelationships()
        {
            var entityType = _modelBuilder.Model.FindEntityType(typeof(IdentityUserClaim<string>));
            Assert.NotNull(entityType);

            var foreignKeys = entityType.GetForeignKeys().ToList();
            Assert.Single(foreignKeys);

            var foreignKey = foreignKeys[0];
            Assert.Equal("UserId", foreignKey.Properties[0].Name);
            Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        }

        [Fact]
        public void UserRoles_HasCorrectCompositeKey()
        {
            var entityType = _modelBuilder.Model.FindEntityType(typeof(IdentityUserRole<string>));
            Assert.NotNull(entityType);

            var key = entityType.FindPrimaryKey();
            Assert.Equal(2, key.Properties.Count);
            Assert.Equal("UserId", key.Properties[0].Name);
            Assert.Equal("RoleId", key.Properties[1].Name);
        }
    }
}
