using System.Linq;
using Xunit;
using IdentityModel;
using System.Text.Json;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class TestUsersTests
    {
        [Fact]
        public void Users_ShouldContainTwoTestUsers()
        {
            var users = TestUsers.Users;
            
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void AliceUser_ShouldHaveCorrectProperties()
        {
            var alice = TestUsers.Users.First(u => u.Username == "alice");
            
            Assert.Equal("818727", alice.SubjectId);
            Assert.Equal("alice", alice.Password);
            Assert.Equal("Alice Smith", alice.Claims.First(c => c.Type == JwtClaimTypes.Name).Value);
            Assert.Equal("Alice", alice.Claims.First(c => c.Type == JwtClaimTypes.GivenName).Value);
            Assert.Equal("Smith", alice.Claims.First(c => c.Type == JwtClaimTypes.FamilyName).Value);
            Assert.Equal("AliceSmith@email.com", alice.Claims.First(c => c.Type == JwtClaimTypes.Email).Value);
            Assert.Equal("true", alice.Claims.First(c => c.Type == JwtClaimTypes.EmailVerified).Value);
            Assert.Equal("http://alice.com", alice.Claims.First(c => c.Type == JwtClaimTypes.WebSite).Value);
        }

        [Fact]
        public void BobUser_ShouldHaveCorrectProperties()
        {
            var bob = TestUsers.Users.First(u => u.Username == "bob");
            
            Assert.Equal("88421113", bob.SubjectId);
            Assert.Equal("bob", bob.Password);
            Assert.Equal("Bob Smith", bob.Claims.First(c => c.Type == JwtClaimTypes.Name).Value);
            Assert.Equal("Bob", bob.Claims.First(c => c.Type == JwtClaimTypes.GivenName).Value);
            Assert.Equal("Smith", bob.Claims.First(c => c.Type == JwtClaimTypes.FamilyName).Value);
            Assert.Equal("BobSmith@email.com", bob.Claims.First(c => c.Type == JwtClaimTypes.Email).Value);
            Assert.Equal("true", bob.Claims.First(c => c.Type == JwtClaimTypes.EmailVerified).Value);
            Assert.Equal("http://bob.com", bob.Claims.First(c => c.Type == JwtClaimTypes.WebSite).Value);
        }

        [Fact]
        public void Users_ShouldHaveValidAddressClaim()
        {
            var user = TestUsers.Users.First();
            var addressClaim = user.Claims.First(c => c.Type == JwtClaimTypes.Address);
            
            var address = JsonSerializer.Deserialize<JsonElement>(addressClaim.Value);
            
            Assert.Equal("One Hacker Way", address.GetProperty("street_address").GetString());
            Assert.Equal("Heidelberg", address.GetProperty("locality").GetString());
            Assert.Equal(69118, address.GetProperty("postal_code").GetInt32());
            Assert.Equal("Germany", address.GetProperty("country").GetString());
        }
    }
}
