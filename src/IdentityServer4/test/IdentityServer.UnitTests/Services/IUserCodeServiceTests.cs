using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class IUserCodeServiceTests
    {
        private readonly Mock<IUserCodeService> _mockUserCodeService;

        public IUserCodeServiceTests()
        {
            _mockUserCodeService = new Mock<IUserCodeService>();
        }

        [Fact]
        public async Task GetGenerator_ShouldReturnUserCodeGenerator()
        {
            // Arrange
            var mockGenerator = new Mock<IUserCodeGenerator>();
            _mockUserCodeService.Setup(x => x.GetGenerator(It.IsAny<string>()))
                .ReturnsAsync(mockGenerator.Object);

            // Act
            var result = await _mockUserCodeService.Object.GetGenerator("numeric");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IUserCodeGenerator>();
            _mockUserCodeService.Verify(x => x.GetGenerator("numeric"), Times.Once);
        }

        [Fact]
        public async Task GetGenerator_WithNullType_ShouldReturnGenerator()
        {
            // Arrange
            var mockGenerator = new Mock<IUserCodeGenerator>();
            _mockUserCodeService.Setup(x => x.GetGenerator(null))
                .ReturnsAsync(mockGenerator.Object);

            // Act
            var result = await _mockUserCodeService.Object.GetGenerator(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IUserCodeGenerator>();
            _mockUserCodeService.Verify(x => x.GetGenerator(null), Times.Once);
        }
    }
}
