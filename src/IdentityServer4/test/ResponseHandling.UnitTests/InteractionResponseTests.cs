using FluentAssertions;
using IdentityServer4.ResponseHandling;
using Xunit;

namespace IdentityServer4.UnitTests.ResponseHandling
{
    public class InteractionResponseTests
    {
        [Fact]
        public void IsLogin_Default_ShouldBeFalse()
        {
            var response = new InteractionResponse();
            response.IsLogin.Should().BeFalse();
        }

        [Fact]
        public void IsConsent_Default_ShouldBeFalse()
        {
            var response = new InteractionResponse();
            response.IsConsent.Should().BeFalse();
        }

        [Fact]
        public void IsError_WhenErrorIsNull_ShouldBeFalse()
        {
            var response = new InteractionResponse();
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public void IsError_WhenErrorIsSet_ShouldBeTrue()
        {
            var response = new InteractionResponse { Error = "error" };
            response.IsError.Should().BeTrue();
        }

        [Fact]
        public void IsRedirect_WhenRedirectUrlIsNull_ShouldBeFalse()
        {
            var response = new InteractionResponse();
            response.IsRedirect.Should().BeFalse();
        }

        [Fact]
        public void IsRedirect_WhenRedirectUrlIsEmpty_ShouldBeFalse()
        {
            var response = new InteractionResponse { RedirectUrl = "" };
            response.IsRedirect.Should().BeFalse();
        }

        [Fact]
        public void IsRedirect_WhenRedirectUrlIsSet_ShouldBeTrue()
        {
            var response = new InteractionResponse { RedirectUrl = "https://example.com" };
            response.IsRedirect.Should().BeTrue();
        }

        [Fact]
        public void ErrorDescription_ShouldBeSettable()
        {
            var response = new InteractionResponse { ErrorDescription = "test error description" };
            response.ErrorDescription.Should().Be("test error description");
        }
    }
}
