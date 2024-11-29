using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using FakeItEasy;
using FluentAssertions;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.UnitTests.Stores
{
    public class ResourceStoreTests
    {
        private readonly IConfigurationDbContext _context;
        private readonly ILogger<ResourceStore> _logger;
        private readonly ResourceStore _store;

        public ResourceStoreTests()
        {
            _context = A.Fake<IConfigurationDbContext>();
            _logger = A.Fake<ILogger<ResourceStore>>();
            _store = new ResourceStore(_context, _logger);
        }

        [Fact]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ResourceStore(null, _logger);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourcesExist_ReturnsMatchingResources()
        {
            // Arrange
            var apiResource = new ApiResource { Name = "test-api" };
            var dbSet = A.Fake<DbSet<ApiResource>>();
            
            A.CallTo(() => _context.ApiResources).Returns(dbSet);
            A.CallTo(() => dbSet.Include(A<string>._))
                .Returns(dbSet);
            A.CallTo(() => dbSet.ToArrayAsync())
                .Returns(new[] { apiResource });

            // Act
            var result = await _store.FindApiResourcesByNameAsync(new[] { "test-api" });

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Name.Should().Be("test-api");
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourcesExist_ReturnsMatchingResources()
        {
            // Arrange
            var identityResource = new IdentityResource { Name = "openid" };
            var dbSet = A.Fake<DbSet<IdentityResource>>();
            
            A.CallTo(() => _context.IdentityResources).Returns(dbSet);
            A.CallTo(() => dbSet.Include(A<string>._))
                .Returns(dbSet);
            A.CallTo(() => dbSet.ToArrayAsync())
                .Returns(new[] { identityResource });

            // Act
            var result = await _store.FindIdentityResourcesByScopeNameAsync(new[] { "openid" });

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Name.Should().Be("openid");
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenScopesExist_ReturnsMatchingScopes()
        {
            // Arrange
            var apiScope = new ApiScope { Name = "test-scope" };
            var dbSet = A.Fake<DbSet<ApiScope>>();
            
            A.CallTo(() => _context.ApiScopes).Returns(dbSet);
            A.CallTo(() => dbSet.Include(A<string>._))
                .Returns(dbSet);
            A.CallTo(() => dbSet.ToArrayAsync())
                .Returns(new[] { apiScope });

            // Act
            var result = await _store.FindApiScopesByNameAsync(new[] { "test-scope" });

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Name.Should().Be("test-scope");
        }

        [Fact]
        public async Task GetAllResourcesAsync_ReturnsAllResources()
        {
            // Arrange
            var identityResource = new IdentityResource { Name = "openid" };
            var apiResource = new ApiResource { Name = "test-api" };
            var apiScope = new ApiScope { Name = "test-scope" };

            var identityDbSet = A.Fake<DbSet<IdentityResource>>();
            var apiDbSet = A.Fake<DbSet<ApiResource>>();
            var scopeDbSet = A.Fake<DbSet<ApiScope>>();

            A.CallTo(() => _context.IdentityResources).Returns(identityDbSet);
            A.CallTo(() => _context.ApiResources).Returns(apiDbSet);
            A.CallTo(() => _context.ApiScopes).Returns(scopeDbSet);

            A.CallTo(() => identityDbSet.Include(A<string>._)).Returns(identityDbSet);
            A.CallTo(() => apiDbSet.Include(A<string>._)).Returns(apiDbSet);
            A.CallTo(() => scopeDbSet.Include(A<string>._)).Returns(scopeDbSet);

            A.CallTo(() => identityDbSet.ToArrayAsync()).Returns(new[] { identityResource });
            A.CallTo(() => apiDbSet.ToArrayAsync()).Returns(new[] { apiResource });
            A.CallTo(() => scopeDbSet.ToArrayAsync()).Returns(new[] { apiScope });

            // Act
            var result = await _store.GetAllResourcesAsync();

            // Assert
            result.Should().NotBeNull();
            result.IdentityResources.Count().Should().Be(1);
            result.ApiResources.Count().Should().Be(1);
            result.ApiScopes.Count().Should().Be(1);
        }
    }
}
