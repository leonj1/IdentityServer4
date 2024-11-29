using Xunit;
using IdentityModel;
using System.Linq;
using System.Security.Claims;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class TestUsersTests
    {
        [Fact]
        public void Users_List_Contains_Expected_Users()
        {
            // Arrange & Act
            var users = TestUsers.Users;

            // Assert
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Username == "alice");
            Assert.Contains(users, u => u.Username == "bob");
        }

        [Theory]
        [InlineData("alice", "818727")]
        [InlineData("bob", "88421113")]
        public void Users_Have_Correct_SubjectIds(string username, string expectedSubjectId)
        {
            // Arrange
            var user = TestUsers.Users.First(u => u.Username == username);

            // Assert
            Assert.Equal(expectedSubjectId, user.SubjectId);
        }

        [Fact]
        public void Alice_Has_Expected_Claims()
        {
            // Arrange
            var alice = TestUsers.Users.First(u => u.Username == "alice");

            // Assert
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.Name && c.Value == "Alice Smith");
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.GivenName && c.Value == "Alice");
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.FamilyName && c.Value == "Smith");
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.Email && c.Value == "AliceSmith@email.com");
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.EmailVerified && c.Value == "true");
            Assert.Contains(alice.Claims, c => c.Type == JwtClaimTypes.WebSite && c.Value == "http://alice.com");
        }

        [Fact]
        public void Bob_Has_Additional_Location_Claim()
        {
            // Arrange
            var bob = TestUsers.Users.First(u => u.Username == "bob");

            // Assert
            var locationClaim = Assert.Single(bob.Claims.Where(c => c.Type == "location"));
            Assert.Equal("somewhere", locationClaim.Value);
        }

        [Fact]
        public void Users_Have_Valid_Address_Claims()
        {
            // Arrange & Act
            var users = TestUsers.Users;

            // Assert
            foreach (var user in users)
            {
                var addressClaim = user.Claims.First(c => c.Type == JwtClaimTypes.Address);
                Assert.Equal(IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json, addressClaim.ValueType);
                Assert.Contains("One Hacker Way", addressClaim.Value);
                Assert.Contains("Heidelberg", addressClaim.Value);
            }
        }
    }
}
