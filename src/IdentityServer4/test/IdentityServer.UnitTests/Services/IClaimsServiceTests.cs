using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class ClaimsServiceTests
    {
        private readonly IClaimsService _sut;
        private readonly ClaimsPrincipal _subject;
        private readonly ResourceValidationResult _resources;
        private readonly ValidatedRequest _request;

        public ClaimsServiceTests()
        {
            _sut = new TestClaimsService();
            _subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
            {
                new Claim("sub", "123"),
                new Claim("name", "Test User")
            }));
            _resources = new ResourceValidationResult();
            _request = new ValidatedRequest();
        }

        [Fact]
        public async Task GetIdentityTokenClaims_ShouldReturnExpectedClaims()
        {
            // Arrange
            var includeAllClaims = true;

            // Act
            var claims = await _sut.GetIdentityTokenClaimsAsync(_subject, _resources, includeAllClaims, _request);

            // Assert
            claims.Should().NotBeNull();
            claims.Should().Contain(x => x.Type == "sub" && x.Value == "123");
            claims.Should().Contain(x => x.Type == "name" && x.Value == "Test User");
        }

        [Fact]
        public async Task GetAccessTokenClaims_ShouldReturnExpectedClaims()
        {
            // Act
            var claims = await _sut.GetAccessTokenClaimsAsync(_subject, _resources, _request);

            // Assert
            claims.Should().NotBeNull();
            claims.Should().Contain(x => x.Type == "sub" && x.Value == "123");
        }

        private class TestClaimsService : IClaimsService
        {
            public Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resources, ValidatedRequest request)
            {
                return Task.FromResult<IEnumerable<Claim>>(subject.Claims);
            }

            public Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resources, bool includeAllIdentityClaims, ValidatedRequest request)
            {
                return Task.FromResult<IEnumerable<Claim>>(subject.Claims);
            }
        }
    }
}
