using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Stores
{
    public class ClientStoreTests
    {
        private readonly Mock<IConfigurationDbContext> _contextMock;
        private readonly Mock<ILogger<ClientStore>> _loggerMock;
        private readonly ClientStore _store;
        private readonly DbSet<Entities.Client> _clientDbSet;

        public ClientStoreTests()
        {
            _contextMock = new Mock<IConfigurationDbContext>();
            _loggerMock = new Mock<ILogger<ClientStore>>();
            _store = new ClientStore(_contextMock.Object, _loggerMock.Object);
            
            _clientDbSet = MockDbSet();
            _contextMock.Setup(x => x.Clients).Returns(_clientDbSet);
        }

        private DbSet<Entities.Client> MockDbSet()
        {
            var mockSet = new Mock<DbSet<Entities.Client>>();
            var client = new Entities.Client
            {
                ClientId = "test-client",
                ClientName = "Test Client"
            };

            var clients = new[] { client }.AsQueryable();

            mockSet.As<IQueryable<Entities.Client>>().Setup(m => m.Provider).Returns(clients.Provider);
            mockSet.As<IQueryable<Entities.Client>>().Setup(m => m.Expression).Returns(clients.Expression);
            mockSet.As<IQueryable<Entities.Client>>().Setup(m => m.ElementType).Returns(clients.ElementType);
            mockSet.As<IQueryable<Entities.Client>>().Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

            return mockSet.Object;
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ReturnsClient()
        {
            // Arrange
            var clientId = "test-client";

            // Act
            var result = await _store.FindClientByIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clientId, result.ClientId);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ReturnsNull()
        {
            // Arrange
            var clientId = "non-existent-client";

            // Act
            var result = await _store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new ClientStore(null, _loggerMock.Object));
            
            Assert.Equal("context", exception.ParamName);
        }
    }
}
