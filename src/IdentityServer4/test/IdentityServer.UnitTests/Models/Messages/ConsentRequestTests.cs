using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Models.Messages
{
    public class ConsentRequestTests
    {
        [Fact]
        public void Constructor_WithAuthorizationRequest_ShouldSetProperties()
        {
            // Arrange
            var client = new Client { ClientId = "client1" };
            var parameters = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.Nonce, "nonce123" },
                { OidcConstants.AuthorizeRequest.Scope, "openid profile email" }
            };
            var authRequest = new AuthorizationRequest { 
                Client = client,
                Parameters = parameters
            };
            var subject = "user123";

            // Act
            var consentRequest = new ConsentRequest(authRequest, subject);

            // Assert
            consentRequest.ClientId.Should().Be("client1");
            consentRequest.Nonce.Should().Be("nonce123");
            consentRequest.ScopesRequested.Should().BeEquivalentTo(new[] { "openid", "profile", "email" });
            consentRequest.Subject.Should().Be("user123");
        }

        [Fact]
        public void Constructor_WithNameValueCollection_ShouldSetProperties()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.ClientId, "client1" },
                { OidcConstants.AuthorizeRequest.Nonce, "nonce123" },
                { OidcConstants.AuthorizeRequest.Scope, "openid profile" }
            };
            var subject = "user123";

            // Act
            var consentRequest = new ConsentRequest(parameters, subject);

            // Assert
            consentRequest.ClientId.Should().Be("client1");
            consentRequest.Nonce.Should().Be("nonce123");
            consentRequest.ScopesRequested.Should().BeEquivalentTo(new[] { "openid", "profile" });
            consentRequest.Subject.Should().Be("user123");
        }

        [Fact]
        public void Id_ShouldBeConsistent_ForSameInput()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.ClientId, "client1" },
                { OidcConstants.AuthorizeRequest.Nonce, "nonce123" },
                { OidcConstants.AuthorizeRequest.Scope, "openid profile" }
            };
            var subject = "user123";

            // Act
            var request1 = new ConsentRequest(parameters, subject);
            var request2 = new ConsentRequest(parameters, subject);

            // Assert
            request1.Id.Should().Be(request2.Id);
        }

        [Fact]
        public void Id_ShouldBeConsistent_WithDifferentScopeOrder()
        {
            // Arrange
            var parameters1 = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.ClientId, "client1" },
                { OidcConstants.AuthorizeRequest.Nonce, "nonce123" },
                { OidcConstants.AuthorizeRequest.Scope, "openid profile" }
            };
            var parameters2 = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.ClientId, "client1" },
                { OidcConstants.AuthorizeRequest.Nonce, "nonce123" },
                { OidcConstants.AuthorizeRequest.Scope, "profile openid" }
            };
            var subject = "user123";

            // Act
            var request1 = new ConsentRequest(parameters1, subject);
            var request2 = new ConsentRequest(parameters2, subject);

            // Assert
            request1.Id.Should().Be(request2.Id);
        }
    }
}
