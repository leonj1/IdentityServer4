using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;
using FluentAssertions;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class ReferenceTokenStoreTests
    {
        private Mock<IReferenceTokenStore> _mockStore;
        private Token _testToken;

        public ReferenceTokenStoreTests()
        {
            _mockStore = new Mock<IReferenceTokenStore>();
            _testToken = new Token
            {
                ClientId = "test_client",
                SubjectId = "test_subject"
            };
        }

        [Fact]
        public async Task StoreReferenceToken_ShouldReturnHandle()
        {
            // Arrange
            var expectedHandle = "test_handle";
            _mockStore.Setup(x => x.StoreReferenceTokenAsync(_testToken))
                .ReturnsAsync(expectedHandle);

            // Act
            var result = await _mockStore.Object.StoreReferenceTokenAsync(_testToken);

            // Assert
            result.Should().Be(expectedHandle);
            _mockStore.Verify(x => x.StoreReferenceTokenAsync(_testToken), Times.Once);
        }

        [Fact]
        public async Task GetReferenceToken_ShouldReturnToken()
        {
            // Arrange
            var handle = "test_handle";
            _mockStore.Setup(x => x.GetReferenceTokenAsync(handle))
                .ReturnsAsync(_testToken);

            // Act
            var result = await _mockStore.Object.GetReferenceTokenAsync(handle);

            // Assert
            result.Should().BeEquivalentTo(_testToken);
            _mockStore.Verify(x => x.GetReferenceTokenAsync(handle), Times.Once);
        }

        [Fact]
        public async Task RemoveReferenceToken_ShouldCallStore()
        {
            // Arrange
            var handle = "test_handle";

            // Act
            await _mockStore.Object.RemoveReferenceTokenAsync(handle);

            // Assert
            _mockStore.Verify(x => x.RemoveReferenceTokenAsync(handle), Times.Once);
        }

        [Fact]
        public async Task RemoveReferenceTokens_ShouldCallStore()
        {
            // Arrange
            var subjectId = "test_subject";
            var clientId = "test_client";

            // Act
            await _mockStore.Object.RemoveReferenceTokensAsync(subjectId, clientId);

            // Assert
            _mockStore.Verify(x => x.RemoveReferenceTokensAsync(subjectId, clientId), Times.Once);
        }
    }
}
