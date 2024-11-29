using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using IdentityServerHost;

namespace IdentityServer4.UnitTests
{
    public class LocalApiControllerTests
    {
        private LocalApiController _controller;

        public LocalApiControllerTests()
        {
            _controller = new LocalApiController();
            
            // Setup ClaimsPrincipal for the controller
            var claims = new List<Claim>
            {
                new Claim("sub", "123"),
                new Claim("name", "Test User"),
                new Claim("role", "admin")
            };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            // Set the User property of ControllerBase
            typeof(ControllerBase)
                .GetProperty("User")
                .SetValue(_controller, principal);
        }

        [Fact]
        public void Get_ShouldReturnAllUserClaims()
        {
            // Act
            var result = _controller.Get() as JsonResult;

            // Assert
            Assert.NotNull(result);
            var claims = result.Value as IEnumerable<object>;
            Assert.NotNull(claims);
            Assert.Equal(3, claims.Count());

            var claimsList = claims.ToList();
            Assert.Contains(claimsList, c => 
                ((dynamic)c).Type == "sub" && 
                ((dynamic)c).Value == "123");
            Assert.Contains(claimsList, c => 
                ((dynamic)c).Type == "name" && 
                ((dynamic)c).Value == "Test User");
            Assert.Contains(claimsList, c => 
                ((dynamic)c).Type == "role" && 
                ((dynamic)c).Value == "admin");
        }
    }
}
