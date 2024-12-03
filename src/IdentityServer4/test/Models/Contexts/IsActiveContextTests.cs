using System;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models.Contexts
{
    public class IsActiveContextTests
    {
        private readonly ClaimsPrincipal _subject;
        private readonly Client _client;
        private readonly string _caller;

        public IsActiveContextTests()
        {
            _subject = new ClaimsPrincipal(new ClaimsIdentity());
            _client = new Client();
            _caller = "test_caller";
        }

        [Fact]
        public void Constructor_WhenValidParameters_ShouldCreateInstance()
        {
            // Act
            var context = new IsActiveContext(_subject, _client, _caller);

            // Assert
            context.Subject.Should().Be(_subject);
            context.Client.Should().Be(_client);
            context.Caller.Should().Be(_caller);
            context.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WhenNullSubject_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new IsActiveContext(null, _client, _caller);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("subject");
        }

        [Fact]
        public void Constructor_WhenNullClient_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new IsActiveContext(_subject, null, _caller);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }

        [Fact]
        public void Constructor_WhenNullCaller_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new IsActiveContext(_subject, _client, null);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("caller");
        }

        [Fact]
        public void IsActive_WhenModified_ShouldUpdateValue()
        {
            // Arrange
            var context = new IsActiveContext(_subject, _client, _caller);

            // Act
            context.IsActive = false;

            // Assert
            context.IsActive.Should().BeFalse();
        }
    }
}
