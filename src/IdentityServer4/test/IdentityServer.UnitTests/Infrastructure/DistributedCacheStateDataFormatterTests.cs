using System;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Infrastructure
{
    public class DistributedCacheStateDataFormatterTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<IDataProtector> _mockProtector;
        private readonly DistributedCacheStateDataFormatter _formatter;
        private readonly string _schemeName = "TestScheme";

        public DistributedCacheStateDataFormatterTests()
        {
            // Setup mocks
            _mockCache = new Mock<IDistributedCache>();
            _mockProtector = new Mock<IDataProtector>();
            var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();

            // Setup data protection
            mockDataProtectionProvider
                .Setup(x => x.CreateProtector(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_mockProtector.Object);
            _mockProtector
                .Setup(x => x.Protect(It.IsAny<byte[]>()))
                .Returns<byte[]>(input => input);
            _mockProtector
                .Setup(x => x.Unprotect(It.IsAny<byte[]>()))
                .Returns<byte[]>(input => input);

            // Setup HTTP context
            var services = new ServiceCollection();
            services.AddSingleton(_mockCache.Object);
            services.AddSingleton(mockDataProtectionProvider.Object);
            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            _httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = httpContext
            };

            _formatter = new DistributedCacheStateDataFormatter(_httpContextAccessor, _schemeName);
        }

        [Fact]
        public void Protect_WithValidData_ShouldStoreInCacheAndReturnProtectedData()
        {
            // Arrange
            var properties = new AuthenticationProperties(
                new Dictionary<string, string> { { "test", "value" } });

            // Act
            var protectedData = _formatter.Protect(properties);

            // Assert
            _mockCache.Verify(x => x.SetString(
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("value")),
                It.IsAny<DistributedCacheEntryOptions>()),
                Times.Once);
            protectedData.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Protect_WithExpirationTime_ShouldSetAbsoluteExpiration()
        {
            // Arrange
            var expirationTime = DateTime.UtcNow.AddHours(1);
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = expirationTime
            };

            // Act
            _formatter.Protect(properties);

            // Assert
            _mockCache.Verify(x => x.SetString(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<DistributedCacheEntryOptions>(o => 
                    o.AbsoluteExpiration == expirationTime)),
                Times.Once);
        }

        [Fact]
        public void Unprotect_WithValidData_ShouldRetrieveFromCache()
        {
            // Arrange
            var key = Guid.NewGuid().ToString();
            var json = "{\"test\":\"value\"}";
            _mockProtector
                .Setup(x => x.Unprotect(It.IsAny<string>()))
                .Returns(key);
            _mockCache
                .Setup(x => x.GetString(It.IsAny<string>()))
                .Returns(json);

            // Act
            var result = _formatter.Unprotect(key);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().ContainKey("test");
            result.Items["test"].Should().Be("value");
        }

        [Fact]
        public void Unprotect_WithNullData_ShouldReturnNull()
        {
            // Act
            var result = _formatter.Unprotect(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Unprotect_WithMissingCacheData_ShouldReturnNull()
        {
            // Arrange
            _mockCache
                .Setup(x => x.GetString(It.IsAny<string>()))
                .Returns((string)null);

            // Act
            var result = _formatter.Unprotect("some-key");

            // Assert
            result.Should().BeNull();
        }
    }
}
