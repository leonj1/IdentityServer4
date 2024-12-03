using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Models.Messages
{
    public class ErrorMessageTests
    {
        [Fact]
        public void ErrorMessage_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var errorMessage = new ErrorMessage
            {
                DisplayMode = "page",
                UiLocales = "en-US",
                Error = "invalid_request",
                ErrorDescription = "The request is missing a required parameter",
                RequestId = "123456",
                RedirectUri = "https://example.com/callback",
                ResponseMode = "form_post",
                ClientId = "client123"
            };

            // Assert
            Assert.Equal("page", errorMessage.DisplayMode);
            Assert.Equal("en-US", errorMessage.UiLocales);
            Assert.Equal("invalid_request", errorMessage.Error);
            Assert.Equal("The request is missing a required parameter", errorMessage.ErrorDescription);
            Assert.Equal("123456", errorMessage.RequestId);
            Assert.Equal("https://example.com/callback", errorMessage.RedirectUri);
            Assert.Equal("form_post", errorMessage.ResponseMode);
            Assert.Equal("client123", errorMessage.ClientId);
        }

        [Fact]
        public void ErrorMessage_Properties_Should_Default_To_Null()
        {
            // Arrange
            var errorMessage = new ErrorMessage();

            // Assert
            Assert.Null(errorMessage.DisplayMode);
            Assert.Null(errorMessage.UiLocales);
            Assert.Null(errorMessage.Error);
            Assert.Null(errorMessage.ErrorDescription);
            Assert.Null(errorMessage.RequestId);
            Assert.Null(errorMessage.RedirectUri);
            Assert.Null(errorMessage.ResponseMode);
            Assert.Null(errorMessage.ClientId);
        }
    }
}
