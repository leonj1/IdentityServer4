using Xunit;
using IdentityServerHost.Quickstart.UI;
using System.Linq;

namespace IdentityServerHost.Tests.Quickstart.Consent
{
    public class ConsentInputModelTests
    {
        [Fact]
        public void ConsentInputModel_Properties_ShouldInitializeCorrectly()
        {
            // Arrange
            var model = new ConsentInputModel
            {
                Button = "yes",
                ScopesConsented = new[] { "scope1", "scope2" },
                RememberConsent = true,
                ReturnUrl = "/return",
                Description = "Test description"
            };

            // Assert
            Assert.Equal("yes", model.Button);
            Assert.Equal(2, model.ScopesConsented.Count());
            Assert.Contains("scope1", model.ScopesConsented);
            Assert.Contains("scope2", model.ScopesConsented);
            Assert.True(model.RememberConsent);
            Assert.Equal("/return", model.ReturnUrl);
            Assert.Equal("Test description", model.Description);
        }

        [Fact]
        public void ConsentInputModel_DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var model = new ConsentInputModel();

            // Assert
            Assert.Null(model.Button);
            Assert.Null(model.ScopesConsented);
            Assert.False(model.RememberConsent);
            Assert.Null(model.ReturnUrl);
            Assert.Null(model.Description);
        }
    }
}
