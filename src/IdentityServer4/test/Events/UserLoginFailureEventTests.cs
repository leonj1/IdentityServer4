using System;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;
using static IdentityServer4.Constants;

namespace IdentityServer4.UnitTests.Events
{
    public class UserLoginFailureEventTests
    {
        [Fact]
        public void UserLoginFailureEvent_Interactive_Should_Set_Correct_Values()
        {
            // Arrange
            var username = "testUser";
            var error = "invalid_credentials";
            var clientId = "test_client";
            
            // Act
            var evt = new UserLoginFailureEvent(username, error, interactive: true, clientId: clientId);

            // Assert
            evt.Username.Should().Be(username);
            evt.ClientId.Should().Be(clientId);
            evt.Endpoint.Should().Be("UI");
            evt.Category.Should().Be(EventCategories.Authentication);
            evt.Name.Should().Be("User Login Failure");
            evt.EventType.Should().Be(EventTypes.Failure);
            evt.Id.Should().Be(EventIds.UserLoginFailure);
            evt.Message.Should().Be(error);
        }

        [Fact]
        public void UserLoginFailureEvent_NonInteractive_Should_Set_Correct_Values()
        {
            // Arrange
            var username = "testUser";
            var error = "invalid_credentials";
            var clientId = "test_client";
            
            // Act
            var evt = new UserLoginFailureEvent(username, error, interactive: false, clientId: clientId);

            // Assert
            evt.Username.Should().Be(username);
            evt.ClientId.Should().Be(clientId);
            evt.Endpoint.Should().Be(EndpointNames.Token);
            evt.Category.Should().Be(EventCategories.Authentication);
            evt.Name.Should().Be("User Login Failure");
            evt.EventType.Should().Be(EventTypes.Failure);
            evt.Id.Should().Be(EventIds.UserLoginFailure);
            evt.Message.Should().Be(error);
        }

        [Fact]
        public void UserLoginFailureEvent_NullClientId_Should_Work()
        {
            // Arrange
            var username = "testUser";
            var error = "invalid_credentials";
            
            // Act
            var evt = new UserLoginFailureEvent(username, error);

            // Assert
            evt.Username.Should().Be(username);
            evt.ClientId.Should().BeNull();
            evt.Endpoint.Should().Be("UI");
        }
    }
}
