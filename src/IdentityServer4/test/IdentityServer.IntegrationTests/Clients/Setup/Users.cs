using System.Collections.Generic;
using IdentityServer4.Test;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    internal static class Users
    {
        public static List<TestUser> Get()
        {
            var users = new List<TestUser>
            {
                new TestUser{SubjectId = "818727", Username = "alice", Password = "alice", 
                    Claims = ClaimHelper.GetClaims("Alice Smith", "Alice", "Smith", "AliceSmith@email.com", true, "Admin", "Geek", "http://alice.com")
                },
                new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob", 
                    Claims = ClaimHelper.GetClaims("Bob Smith", "Bob", "Smith", "BobSmith@email.com", true, "Developer", "Geek", "http://bob.com")
                },
                new TestUser{SubjectId = "88421113", Username = "bob_no_password", 
                    Claims = ClaimHelper.GetClaims("Bob Smith", "Bob", "Smith", "BobSmith@email.com", true, "Developer", "Geek", "http://bob.com")
                }
            };

            return users;
        }
    }
}
