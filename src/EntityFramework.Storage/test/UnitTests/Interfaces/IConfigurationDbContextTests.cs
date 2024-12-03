using System;
using System.Linq;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;

namespace IdentityServer4.EntityFramework.UnitTests.Interfaces
{
    public class IConfigurationDbContextTests
    {
        private readonly Mock<IConfigurationDbContext> _mockContext;

        public IConfigurationDbContextTests()
        {
            _mockContext = new Mock<IConfigurationDbContext>();
        }

        [Fact]
        public void IConfigurationDbContext_Should_Implement_IDisposable()
        {
            // Assert
            Assert.True(typeof(IDisposable).IsAssignableFrom(typeof(IConfigurationDbContext)));
        }

        [Fact]
        public void IConfigurationDbContext_Should_Have_Required_DbSets()
        {
            // Arrange & Act
            var properties = typeof(IConfigurationDbContext).GetProperties();

            // Assert
            Assert.Contains(properties, p => p.Name == "Clients" && p.PropertyType == typeof(DbSet<Client>));
            Assert.Contains(properties, p => p.Name == "ClientCorsOrigins" && p.PropertyType == typeof(DbSet<ClientCorsOrigin>));
            Assert.Contains(properties, p => p.Name == "IdentityResources" && p.PropertyType == typeof(DbSet<IdentityResource>));
            Assert.Contains(properties, p => p.Name == "ApiResources" && p.PropertyType == typeof(DbSet<ApiResource>));
            Assert.Contains(properties, p => p.Name == "ApiScopes" && p.PropertyType == typeof(DbSet<ApiScope>));
        }

        [Fact]
        public void Dispose_Should_Be_Implemented()
        {
            // Arrange
            _mockContext.Setup(x => x.Dispose());

            // Act
            _mockContext.Object.Dispose();

            // Assert
            _mockContext.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
