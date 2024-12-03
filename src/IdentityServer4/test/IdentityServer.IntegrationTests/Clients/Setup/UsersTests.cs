using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using IdentityServer4;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class UsersTests
    {
        [Fact]
        public void Get_ShouldReturnCorrectNumberOfUsers()
        {
            var users = Users.Get();
            users.Should().HaveCount(3);
        }

        [Fact]
        public void Alice_ShouldHaveCorrectProperties()
        {
            var users = Users.Get();
            var alice = users.First(u => u.Username == "alice");

            alice.SubjectId.Should().Be("818727");
            alice.Password.Should().Be("alice");
            alice.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "Alice Smith");
            alice.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "Admin");
            alice.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "Geek");
        }

        [Fact]
        public void Bob_ShouldHaveCorrectProperties()
        {
            var users = Users.Get();
            var bob = users.First(u => u.Username == "bob");

            bob.SubjectId.Should().Be("88421113");
            bob.Password.Should().Be("bob");
            bob.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "Bob Smith");
            bob.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "Developer");
            bob.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "Geek");
        }

        [Fact]
        public void BobNoPassword_ShouldHaveCorrectProperties()
        {
            var users = Users.Get();
            var bobNoPass = users.First(u => u.Username == "bob_no_password");

            bobNoPass.SubjectId.Should().Be("88421113");
            bobNoPass.Password.Should().BeNull();
            bobNoPass.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "Bob Smith");
            bobNoPass.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "Developer");
        }

        [Fact]
        public void Users_ShouldHaveValidAddressClaims()
        {
            var users = Users.Get();
            
            foreach (var user in users)
            {
                var addressClaim = user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Address);
                addressClaim.Should().NotBeNull();
                addressClaim.ValueType.Should().Be(IdentityServerConstants.ClaimValueTypes.Json);
                addressClaim.Value.Should().Contain("One Hacker Way");
                addressClaim.Value.Should().Contain("Heidelberg");
            }
        }
    }
}
