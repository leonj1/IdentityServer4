using IdentityServer4.Models;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationRequestDefaultConstructorTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeCorrectly()
        {
            // Act
            var request = new AuthorizationRequest();

            // Assert
            Assert.NotNull(request.Parameters);
            Assert.Empty(request.Parameters);
            Assert.NotNull(request.RequestObjectValues);
            Assert.Empty(request.RequestObjectValues);
            Assert.Empty(request.PromptModes);
        }
    }
}
