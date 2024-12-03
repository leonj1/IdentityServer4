using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Xunit;
using System.Linq;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class SecurityStampValidatorCallbackTests
    {
        [Fact]
        public async Task UpdatePrincipal_WhenNewPrincipalMissingClaims_ShouldPreserveOriginalClaims()
        {
            // Arrange
            var originalClaims = new[]
            {
                new Claim("idp", "local"),
                new Claim("auth_time", "123"),
                new Claim("amr", "pwd"),
                new Claim("name", "bob")
            };
            
            var newClaims = new[]
            {
                new Claim("name", "bob"),
                new Claim("email", "bob@example.com")
            };

            var currentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(originalClaims));
            var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(newClaims));

            var context = new SecurityStampRefreshingPrincipalContext
            {
                CurrentPrincipal = currentPrincipal,
                NewPrincipal = newPrincipal
            };

            // Act
            await SecurityStampValidatorCallback.UpdatePrincipal(context);

            // Assert
            var resultingClaims = context.NewPrincipal.Claims.ToList();
            Assert.Contains(resultingClaims, c => c.Type == "idp" && c.Value == "local");
            Assert.Contains(resultingClaims, c => c.Type == "auth_time" && c.Value == "123");
            Assert.Contains(resultingClaims, c => c.Type == "amr" && c.Value == "pwd");
            Assert.Contains(resultingClaims, c => c.Type == "name" && c.Value == "bob");
            Assert.Contains(resultingClaims, c => c.Type == "email" && c.Value == "bob@example.com");
            Assert.Equal(5, resultingClaims.Count);
        }
    }
}
