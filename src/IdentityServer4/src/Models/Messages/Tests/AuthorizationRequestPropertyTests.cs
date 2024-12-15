using IdentityServer4.Models;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationRequestPropertyTests
    {
        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var request = new AuthorizationRequest();
            var client = new Client { ClientId = "test_client" };
            var redirectUri = "https://test.com/callback";
            var displayMode = "page";
            var uiLocales = "en-US";
            var idp = "google";
            var tenant = "test_tenant";
            var loginHint = "test@example.com";
            var promptModes = new[] { "login", "consent" };
            var acrValues = new[] { "value1", "value2" };

            // Act
            request.Client = client;
            request.RedirectUri = redirectUri;
            request.DisplayMode = displayMode;
            request.UiLocales = uiLocales;
            request.IdP = idp;
            request.Tenant = tenant;
            request.LoginHint = loginHint;
            request.PromptModes = promptModes;
            request.AcrValues = acrValues;

            // Assert
            Assert.Equal(client, request.Client);
            Assert.Equal(redirectUri, request.RedirectUri);
            Assert.Equal(displayMode, request.DisplayMode);
            Assert.Equal(uiLocales, request.UiLocales);
            Assert.Equal(idp, request.IdP);
            Assert.Equal(tenant, request.Tenant);
            Assert.Equal(loginHint, request.LoginHint);
            Assert.Equal(promptModes, request.PromptModes);
            Assert.Equal(acrValues, request.AcrValues);
        }
    }
}
