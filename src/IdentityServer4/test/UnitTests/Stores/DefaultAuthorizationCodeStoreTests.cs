using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class DefaultAuthorizationCodeStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _mockPersistedGrantStore;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerationService;
        private readonly Mock<ILogger<DefaultAuthorizationCodeStore>> _mockLogger;
        private readonly DefaultAuthorizationCodeStore _subject;
        private readonly IPersistentGrantSerializer _serializer;

        public DefaultAuthorizationCodeStoreTests()
        {
            _mockPersistedGrantStore = new Mock<IPersistedGrantStore>();
            _mockHandleGenerationService = new Mock<IHandleGenerationService>();
            _mockLogger = new Mock<ILogger<DefaultAuthorizationCodeStore>>();
            _serializer = new PersistentGrantSerializer();
            
            _subject = new DefaultAuthorizationCodeStore(
                _mockPersistedGrantStore.Object,
                _serializer,
                _mockHandleGenerationService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_WhenCodeIsValid_ShouldStoreCode()
        {
            // Arrange
            var handle = "test_handle";
            _mockHandleGenerationService.Setup(x => x.GenerateAsync()).ReturnsAsync(handle);

            var code = new AuthorizationCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim("sub", "123")
                })),
                SessionId = "session1",
                Description = "test code"
            };

            // Act
            var result = await _subject.StoreAuthorizationCodeAsync(code);

            // Assert
            result.Should().Be(handle);
            _mockPersistedGrantStore.Verify(x => x.StoreAsync(It.IsAny<PersistedGrant>()), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizationCodeAsync_WhenCodeExists_ShouldReturnCode()
        {
            // Arrange
            var handle = "test_handle";
            var code = new AuthorizationCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim("sub", "123")
                }))
            };

            var persistedGrant = new PersistedGrant
            {
                Key = handle,
                Type = IdentityServerConstants.PersistedGrantTypes.AuthorizationCode,
                Data = _serializer.Serialize(code)
            };

            _mockPersistedGrantStore.Setup(x => x.GetAsync(handle)).ReturnsAsync(persistedGrant);

            // Act
            var result = await _subject.GetAuthorizationCodeAsync(handle);

            // Assert
            result.Should().NotBeNull();
            result.ClientId.Should().Be(code.ClientId);
            result.Subject.FindFirst("sub").Value.Should().Be("123");
        }

        [Fact]
        public async Task RemoveAuthorizationCodeAsync_WhenCodeExists_ShouldRemoveCode()
        {
            // Arrange
            var handle = "test_handle";

            // Act
            await _subject.RemoveAuthorizationCodeAsync(handle);

            // Assert
            _mockPersistedGrantStore.Verify(x => x.RemoveAsync(handle), Times.Once);
        }
    }
}
