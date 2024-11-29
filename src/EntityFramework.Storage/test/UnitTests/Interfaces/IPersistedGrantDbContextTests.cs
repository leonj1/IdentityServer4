using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Interfaces
{
    public class IPersistedGrantDbContextTests
    {
        private readonly Mock<IPersistedGrantDbContext> _mockContext;
        private readonly Mock<DbSet<PersistedGrant>> _mockPersistedGrants;
        private readonly Mock<DbSet<DeviceFlowCodes>> _mockDeviceFlowCodes;

        public IPersistedGrantDbContextTests()
        {
            _mockContext = new Mock<IPersistedGrantDbContext>();
            _mockPersistedGrants = new Mock<DbSet<PersistedGrant>>();
            _mockDeviceFlowCodes = new Mock<DbSet<DeviceFlowCodes>>();
        }

        [Fact]
        public void PersistedGrants_Property_Should_Be_Accessible()
        {
            // Arrange
            _mockContext.Setup(x => x.PersistedGrants)
                .Returns(_mockPersistedGrants.Object);

            // Act
            var result = _mockContext.Object.PersistedGrants;

            // Assert
            Assert.NotNull(result);
            _mockContext.VerifyGet(x => x.PersistedGrants);
        }

        [Fact]
        public void DeviceFlowCodes_Property_Should_Be_Accessible()
        {
            // Arrange
            _mockContext.Setup(x => x.DeviceFlowCodes)
                .Returns(_mockDeviceFlowCodes.Object);

            // Act
            var result = _mockContext.Object.DeviceFlowCodes;

            // Assert
            Assert.NotNull(result);
            _mockContext.VerifyGet(x => x.DeviceFlowCodes);
        }

        [Fact]
        public async Task SaveChangesAsync_Should_Return_Success()
        {
            // Arrange
            const int expectedResult = 1;
            _mockContext.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _mockContext.Object.SaveChangesAsync();

            // Assert
            Assert.Equal(expectedResult, result);
            _mockContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Dispose_Should_Not_Throw()
        {
            // Act & Assert
            var exception = Record.Exception(() => _mockContext.Object.Dispose());
            Assert.Null(exception);
        }
    }
}
