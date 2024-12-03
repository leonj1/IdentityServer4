using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Xunit;
using IdentityServer.UnitTests.Common;

namespace IdentityServer.UnitTests.Services
{
    public class IUserSessionTests
    {
        private readonly MockUserSession _userSession;

        public IUserSessionTests()
        {
            _userSession = new MockUserSession();
        }

        [Fact]
        public async Task CreateSessionId_Should_Return_NonEmpty_String()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "test")
            }));
            var properties = new AuthenticationProperties();

            // Act
            var sessionId = await _userSession.CreateSessionIdAsync(principal, properties);

            // Assert
            sessionId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUser_Should_Return_User()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "test")
            }));
            _userSession.User = principal;

            // Act
            var user = await _userSession.GetUserAsync();

            // Assert
            user.Should().NotBeNull();
            user.Identity.Name.Should().Be("test");
        }

        [Fact]
        public async Task AddClientId_Should_Add_To_ClientList()
        {
            // Arrange
            string clientId = "test_client";

            // Act
            await _userSession.AddClientIdAsync(clientId);
            var clients = await _userSession.GetClientListAsync();

            // Assert
            clients.Should().Contain(clientId);
        }

        [Fact]
        public async Task GetClientList_Should_Return_Empty_When_No_Clients()
        {
            // Act
            var clients = await _userSession.GetClientListAsync();

            // Assert
            clients.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSessionId_Should_Return_Null_When_Not_Set()
        {
            // Act
            var sessionId = await _userSession.GetSessionIdAsync();

            // Assert
            sessionId.Should().BeNull();
        }
    }
}
