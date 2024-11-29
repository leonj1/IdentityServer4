using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class ReturnUrlParserTests
    {
        private readonly Mock<IReturnUrlParser> _mockParser1;
        private readonly Mock<IReturnUrlParser> _mockParser2;
        private readonly ReturnUrlParser _target;

        public ReturnUrlParserTests()
        {
            _mockParser1 = new Mock<IReturnUrlParser>();
            _mockParser2 = new Mock<IReturnUrlParser>();
            
            var parsers = new List<IReturnUrlParser> 
            { 
                _mockParser1.Object, 
                _mockParser2.Object 
            };
            
            _target = new ReturnUrlParser(parsers);
        }

        [Fact]
        public async Task ParseAsync_WhenFirstParserReturnsResult_ShouldReturnThatResult()
        {
            // Arrange
            var expectedRequest = new AuthorizationRequest();
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.ParseAsync(returnUrl))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _target.ParseAsync(returnUrl);

            // Assert
            result.Should().BeSameAs(expectedRequest);
            _mockParser2.Verify(x => x.ParseAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ParseAsync_WhenFirstParserReturnsNull_ShouldTrySecondParser()
        {
            // Arrange
            var expectedRequest = new AuthorizationRequest();
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.ParseAsync(returnUrl))
                .ReturnsAsync((AuthorizationRequest)null);
            _mockParser2.Setup(x => x.ParseAsync(returnUrl))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _target.ParseAsync(returnUrl);

            // Assert
            result.Should().BeSameAs(expectedRequest);
            _mockParser1.Verify(x => x.ParseAsync(returnUrl), Times.Once);
            _mockParser2.Verify(x => x.ParseAsync(returnUrl), Times.Once);
        }

        [Fact]
        public async Task ParseAsync_WhenAllParsersReturnNull_ShouldReturnNull()
        {
            // Arrange
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.ParseAsync(returnUrl))
                .ReturnsAsync((AuthorizationRequest)null);
            _mockParser2.Setup(x => x.ParseAsync(returnUrl))
                .ReturnsAsync((AuthorizationRequest)null);

            // Act
            var result = await _target.ParseAsync(returnUrl);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IsValidReturnUrl_WhenFirstParserReturnsTrue_ShouldReturnTrue()
        {
            // Arrange
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.IsValidReturnUrl(returnUrl))
                .Returns(true);

            // Act
            var result = _target.IsValidReturnUrl(returnUrl);

            // Assert
            result.Should().BeTrue();
            _mockParser2.Verify(x => x.IsValidReturnUrl(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void IsValidReturnUrl_WhenFirstParserReturnsFalse_ShouldTrySecondParser()
        {
            // Arrange
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.IsValidReturnUrl(returnUrl))
                .Returns(false);
            _mockParser2.Setup(x => x.IsValidReturnUrl(returnUrl))
                .Returns(true);

            // Act
            var result = _target.IsValidReturnUrl(returnUrl);

            // Assert
            result.Should().BeTrue();
            _mockParser1.Verify(x => x.IsValidReturnUrl(returnUrl), Times.Once);
            _mockParser2.Verify(x => x.IsValidReturnUrl(returnUrl), Times.Once);
        }

        [Fact]
        public void IsValidReturnUrl_WhenAllParsersReturnFalse_ShouldReturnFalse()
        {
            // Arrange
            var returnUrl = "http://valid.url";

            _mockParser1.Setup(x => x.IsValidReturnUrl(returnUrl))
                .Returns(false);
            _mockParser2.Setup(x => x.IsValidReturnUrl(returnUrl))
                .Returns(false);

            // Act
            var result = _target.IsValidReturnUrl(returnUrl);

            // Assert
            result.Should().BeFalse();
        }
    }
}
