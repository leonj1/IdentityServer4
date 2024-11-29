using System;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class AuthenticationPropertiesExtensionsTests
    {
        private readonly AuthenticationProperties _properties;

        public AuthenticationPropertiesExtensionsTests()
        {
            _properties = new AuthenticationProperties();
        }

        [Fact]
        public void GetSetSessionId_Should_WorkCorrectly()
        {
            // Act
            _properties.SetSessionId("123");
            var sessionId = _properties.GetSessionId();

            // Assert
            sessionId.Should().Be("123");
        }

        [Fact]
        public void GetSessionId_WhenNotSet_ShouldReturnNull()
        {
            // Act
            var sessionId = _properties.GetSessionId();

            // Assert
            sessionId.Should().BeNull();
        }

        [Fact]
        public void GetClientList_WhenEmpty_ShouldReturnEmptyList()
        {
            // Act
            var clients = _properties.GetClientList();

            // Assert
            clients.Should().BeEmpty();
        }

        [Fact]
        public void AddClientId_ShouldAddToList()
        {
            // Act
            _properties.AddClientId("client1");
            _properties.AddClientId("client2");
            var clients = _properties.GetClientList();

            // Assert
            clients.Should().HaveCount(2);
            clients.Should().Contain(new[] { "client1", "client2" });
        }

        [Fact]
        public void AddClientId_WhenDuplicate_ShouldNotAddAgain()
        {
            // Act
            _properties.AddClientId("client1");
            _properties.AddClientId("client1");
            var clients = _properties.GetClientList();

            // Assert
            clients.Should().HaveCount(1);
            clients.Should().Contain("client1");
        }

        [Fact]
        public void RemoveClientList_ShouldClearList()
        {
            // Arrange
            _properties.AddClientId("client1");
            _properties.AddClientId("client2");

            // Act
            _properties.RemoveClientList();
            var clients = _properties.GetClientList();

            // Assert
            clients.Should().BeEmpty();
        }

        [Fact]
        public void AddClientId_WhenNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => _properties.AddClientId(null);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
