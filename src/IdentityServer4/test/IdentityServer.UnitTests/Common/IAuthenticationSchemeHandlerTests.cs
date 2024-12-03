using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Common
{
    public class IAuthenticationSchemeHandlerTests
    {
        [Fact]
        public void Interface_Should_Be_Internal()
        {
            // Arrange
            var type = typeof(IAuthenticationSchemeHandler);

            // Assert
            Assert.True(type.IsInterface);
            Assert.True(type.IsNotPublic);
        }
    }
}
