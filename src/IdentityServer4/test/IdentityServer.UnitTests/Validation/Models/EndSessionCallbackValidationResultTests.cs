using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class EndSessionCallbackValidationResultTests
    {
        [Fact]
        public void Should_Initialize_With_Default_Values()
        {
            var result = new EndSessionCallbackValidationResult();
            
            result.IsError.Should().BeFalse();
            result.Error.Should().BeNull();
            result.FrontChannelLogoutUrls.Should().BeNull();
        }

        [Fact]
        public void Should_Set_And_Get_FrontChannelLogoutUrls()
        {
            var urls = new[] { "https://client1/signout", "https://client2/signout" };
            var result = new EndSessionCallbackValidationResult
            {
                FrontChannelLogoutUrls = urls
            };

            result.FrontChannelLogoutUrls.Should().BeEquivalentTo(urls);
        }

        [Fact]
        public void Should_Inherit_From_ValidationResult()
        {
            var result = new EndSessionCallbackValidationResult
            {
                IsError = true,
                Error = "test_error"
            };

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("test_error");
        }
    }
}
