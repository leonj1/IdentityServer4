using Xunit;
using IdentityServerHost.Quickstart.UI;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class ConsentInputModelTests
    {
        [Fact]
        public void ConsentInputModel_Properties_SetAndGet_Correctly()
        {
            // Arrange
            var model = new ConsentInputModel
            {
                Button = "accept",
                ScopesConsented = new List<string> { "scope1", "scope2" },
                RememberConsent = true,
                ReturnUrl = "/return",
                Description = "Test description"
            };

            // Act & Assert
            Assert.Equal("accept", model.Button);
            Assert.Equal(2, model.ScopesConsented.Count());
            Assert.Contains("scope1", model.ScopesConsented);
            Assert.Contains("scope2", model.ScopesConsented);
            Assert.True(model.RememberConsent);
            Assert.Equal("/return", model.ReturnUrl);
            Assert.Equal("Test description", model.Description);
        }

        [Fact]
        public void ConsentInputModel_DefaultValues_AreCorrect()
        {
            // Arrange
            var model = new ConsentInputModel();

            // Act & Assert
            Assert.Null(model.Button);
            Assert.Null(model.ScopesConsented);
            Assert.False(model.RememberConsent);
            Assert.Null(model.ReturnUrl);
            Assert.Null(model.Description);
        }
    }
}
