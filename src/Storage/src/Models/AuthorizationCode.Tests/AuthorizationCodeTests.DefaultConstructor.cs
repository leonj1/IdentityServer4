using System;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationCodeTests
    {
        [Fact]
        public void AuthorizationCode_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var code = new AuthorizationCode();

            // Assert
            Assert.NotEqual(default(DateTime), code.CreationTime);
            Assert.NotNull(code.Properties);
            Assert.Empty(code.Properties);
        }
    }
}
