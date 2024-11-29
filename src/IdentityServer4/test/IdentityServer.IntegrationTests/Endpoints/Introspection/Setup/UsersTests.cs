using System.Linq;
using System.Collections.Generic;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    public class UsersTests
    {
        [Fact]
        public void Get_ShouldReturnExpectedTestUsers()
        {
            // Act
            var users = Users.Get();

            // Assert
            Assert.NotNull(users);
            Assert.Single(users);

            var user = users.First();
            Assert.Equal("1", user.SubjectId);
            Assert.Equal("bob", user.Username);
            Assert.Equal("bob", user.Password);
        }
    }
}
