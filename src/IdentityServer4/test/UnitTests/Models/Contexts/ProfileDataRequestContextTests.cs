using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models.Contexts
{
    public class ProfileDataRequestContextTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstance_WithEmptyIssuedClaims()
        {
            var context = new ProfileDataRequestContext();
            
            context.IssuedClaims.Should().NotBeNull();
            context.IssuedClaims.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_WithValidInputs_CreatesInstance()
        {
            // Arrange
            var subject = new ClaimsPrincipal();
            var client = new Client();
            var caller = "TestCaller";
            var requestedClaimTypes = new[] { "claim1", "claim2" };

            // Act
            var context = new ProfileDataRequestContext(subject, client, caller, requestedClaimTypes);

            // Assert
            context.Subject.Should().BeSameAs(subject);
            context.Client.Should().BeSameAs(client);
            context.Caller.Should().Be(caller);
            context.RequestedClaimTypes.Should().BeEquivalentTo(requestedClaimTypes);
            context.IssuedClaims.Should().NotBeNull();
            context.IssuedClaims.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_WithNullSubject_ThrowsArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal subject = null;
            var client = new Client();
            var caller = "TestCaller";
            var requestedClaimTypes = new[] { "claim1" };

            // Act & Assert
            Action act = () => new ProfileDataRequestContext(subject, client, caller, requestedClaimTypes);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("subject");
        }

        [Fact]
        public void ParameterizedConstructor_WithNullClient_ThrowsArgumentNullException()
        {
            // Arrange
            var subject = new ClaimsPrincipal();
            Client client = null;
            var caller = "TestCaller";
            var requestedClaimTypes = new[] { "claim1" };

            // Act & Assert
            Action act = () => new ProfileDataRequestContext(subject, client, caller, requestedClaimTypes);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }

        [Fact]
        public void ParameterizedConstructor_WithNullCaller_ThrowsArgumentNullException()
        {
            // Arrange
            var subject = new ClaimsPrincipal();
            var client = new Client();
            string caller = null;
            var requestedClaimTypes = new[] { "claim1" };

            // Act & Assert
            Action act = () => new ProfileDataRequestContext(subject, client, caller, requestedClaimTypes);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("caller");
        }

        [Fact]
        public void ParameterizedConstructor_WithNullRequestedClaimTypes_ThrowsArgumentNullException()
        {
            // Arrange
            var subject = new ClaimsPrincipal();
            var client = new Client();
            var caller = "TestCaller";
            IEnumerable<string> requestedClaimTypes = null;

            // Act & Assert
            Action act = () => new ProfileDataRequestContext(subject, client, caller, requestedClaimTypes);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("requestedClaimTypes");
        }
    }
}
