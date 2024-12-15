using IdentityModel;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System;

namespace IdentityServer4.Test
{
    /// <summary>
    /// Store for test users
    /// </summary>
    public class TestUserStore
    {
        private List<TestUser> _users = new List<TestUser>();

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidateUser(string username, string password)
        {
            return _users.Any(u => u.Username == username && u.Password == password);
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="subjectId">The subject ID.</param>
        /// <returns></returns>
        public TestUser FindUser(string subjectId)
        {
            return _users.FirstOrDefault(u => u.SubjectId == subjectId);
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public TestUser FindUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddUser(TestUser user)
        {
            _users.Add(user);
        }
    }

    /// <summary>
    /// Represents a test user.
    /// </summary>
    public class TestUser
    {
        /// <summary>
        /// Gets or sets the subject ID.
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider subject ID.
        /// </summary>
        public string ProviderSubjectId { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public List<Claim> Claims { get; set; }
    }
}
