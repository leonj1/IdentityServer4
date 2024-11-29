using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Logging.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Logging.Models
{
    public class AuthorizeResponseLogTests
    {
        [Fact]
        public void Constructor_Should_Set_Properties_Correctly()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Request = new ValidatedAuthorizeRequest
                {
                    Client = new IdentityServer4.Models.Client
                    {
                        ClientId = "test_client"
                    },
                    Subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("sub", "123")
                    }))
                },
                RedirectUri = "https://test.com",
                State = "test_state",
                Scope = "openid profile",
                Error = "test_error",
                ErrorDescription = "test_error_description"
            };

            // Act
            var log = new AuthorizeResponseLog(response);

            // Assert
            log.ClientId.Should().Be("test_client");
            log.SubjectId.Should().Be("123");
            log.RedirectUri.Should().Be("https://test.com");
            log.State.Should().Be("test_state");
            log.Scope.Should().Be("openid profile");
            log.Error.Should().Be("test_error");
            log.ErrorDescription.Should().Be("test_error_description");
        }

        [Fact]
        public void ToString_Should_Return_Serialized_Log()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Request = new ValidatedAuthorizeRequest
                {
                    Client = new IdentityServer4.Models.Client
                    {
                        ClientId = "test_client"
                    }
                }
            };
            var log = new AuthorizeResponseLog(response);

            // Act
            var result = log.ToString();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("test_client");
        }

        [Fact]
        public void Constructor_Should_Handle_Null_Request()
        {
            // Arrange
            var response = new AuthorizeResponse();

            // Act
            var log = new AuthorizeResponseLog(response);

            // Assert
            log.ClientId.Should().BeNull();
            log.SubjectId.Should().BeNull();
        }
    }
}
