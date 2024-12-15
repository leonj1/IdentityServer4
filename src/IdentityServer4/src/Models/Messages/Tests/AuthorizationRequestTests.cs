using IdentityServer4.Models;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationRequestTests
    {
        [Fact]
        public void RequestObjectValues_ShouldAllowAddingValues()
        {
            // Arrange
            var request = new AuthorizationRequest();
            var key = "test_key";
            var value = "test_value";

            // Act
            request.RequestObjectValues.Add(key, value);

            // Assert
            Assert.Single(request.RequestObjectValues);
            Assert.Equal(value, request.RequestObjectValues[key]);
        }
    }
}
