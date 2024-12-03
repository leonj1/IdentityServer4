using System;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class UserLoginSuccessEventTests
    {
        [Fact]
        public void UserLoginSuccessEvent_Constructor_WithProvider_ShouldSetProperties()
        {
            // Arrange
            var provider = "TestProvider";
            var providerUserId = "TestProviderUserId";
            var subjectId = "TestSubjectId";
            var name = "TestName";
            var interactive = true;
            var clientId = "TestClientId";

            // Act
            var evt = new UserLoginSuccessEvent(provider, providerUserId, subjectId, name, interactive, clientId);

            // Assert
            evt.Provider.Should().Be(provider);
            evt.ProviderUserId.Should().Be(providerUserId);
            evt.SubjectId.Should().Be(subjectId);
            evt.DisplayName.Should().Be(name);
            evt.Endpoint.Should().Be("UI");
            evt.ClientId.Should().Be(clientId);
        }

        [Fact]
        public void UserLoginSuccessEvent_Constructor_WithUsername_ShouldSetProperties()
        {
            // Arrange
            var username = "TestUsername";
            var subjectId = "TestSubjectId";
            var name = "TestName";
            var interactive = false;
            var clientId = "TestClientId";

            // Act
            var evt = new UserLoginSuccessEvent(username, subjectId, name, interactive, clientId);

            // Assert
            evt.Username.Should().Be(username);
            evt.SubjectId.Should().Be(subjectId);
            evt.DisplayName.Should().Be(name);
            evt.Endpoint.Should().Be("Token");
            evt.ClientId.Should().Be(clientId);
        }

        [Fact]
        public void UserLoginSuccessEvent_BaseProperties_ShouldBeSet()
        {
            // Arrange & Act
            var evt = new UserLoginSuccessEvent("username", "subjectId", "name");

            // Assert
            evt.Category.Should().Be("Authentication");
            evt.Name.Should().Be("User Login Success");
            evt.EventType.Should().Be("Success");
            evt.Id.Should().Be(1000);
        }
    }
}
