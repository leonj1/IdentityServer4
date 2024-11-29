using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class TestUserStoreTests
    {
        private readonly TestUserStore _subject;
        private readonly List<TestUser> _users;

        public TestUserStoreTests()
        {
            _users = new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "123",
                    Username = "test",
                    Password = "password",
                    Claims = new List<Claim> { new Claim("name", "Test User") },
                    ProviderName = "test_provider",
                    ProviderSubjectId = "test_provider_id"
                }
            };
            _subject = new TestUserStore(_users);
        }

        [Fact]
        public void ValidateCredentials_WithValidCredentials_ReturnsTrue()
        {
            var result = _subject.ValidateCredentials("test", "password");
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateCredentials_WithInvalidPassword_ReturnsFalse()
        {
            var result = _subject.ValidateCredentials("test", "wrong");
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateCredentials_WithInvalidUsername_ReturnsFalse()
        {
            var result = _subject.ValidateCredentials("wrong", "password");
            result.Should().BeFalse();
        }

        [Fact]
        public void FindBySubjectId_WithValidId_ReturnsUser()
        {
            var user = _subject.FindBySubjectId("123");
            user.Should().NotBeNull();
            user.Username.Should().Be("test");
        }

        [Fact]
        public void FindBySubjectId_WithInvalidId_ReturnsNull()
        {
            var user = _subject.FindBySubjectId("wrong");
            user.Should().BeNull();
        }

        [Fact]
        public void FindByUsername_WithValidUsername_ReturnsUser()
        {
            var user = _subject.FindByUsername("test");
            user.Should().NotBeNull();
            user.SubjectId.Should().Be("123");
        }

        [Fact]
        public void FindByUsername_WithInvalidUsername_ReturnsNull()
        {
            var user = _subject.FindByUsername("wrong");
            user.Should().BeNull();
        }

        [Fact]
        public void FindByExternalProvider_WithValidProvider_ReturnsUser()
        {
            var user = _subject.FindByExternalProvider("test_provider", "test_provider_id");
            user.Should().NotBeNull();
            user.SubjectId.Should().Be("123");
        }

        [Fact]
        public void FindByExternalProvider_WithInvalidProvider_ReturnsNull()
        {
            var user = _subject.FindByExternalProvider("wrong", "wrong");
            user.Should().BeNull();
        }

        [Fact]
        public void AutoProvisionUser_CreatesNewUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "New User"),
                new Claim(ClaimTypes.GivenName, "New"),
                new Claim(ClaimTypes.FamilyName, "User")
            };

            var user = _subject.AutoProvisionUser("new_provider", "new_id", claims);

            user.Should().NotBeNull();
            user.ProviderName.Should().Be("new_provider");
            user.ProviderSubjectId.Should().Be("new_id");
            user.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "New User");
            _users.Should().Contain(user);
        }

        [Fact]
        public void AutoProvisionUser_WithoutName_UsesFirstAndLastName()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, "New"),
                new Claim(ClaimTypes.FamilyName, "User")
            };

            var user = _subject.AutoProvisionUser("new_provider", "new_id", claims);

            user.Should().NotBeNull();
            user.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "New User");
        }
    }
}
