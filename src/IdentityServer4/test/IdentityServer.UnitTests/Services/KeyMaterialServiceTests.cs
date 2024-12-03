using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class KeyMaterialServiceTests
    {
        private class TestKeyMaterialService : IKeyMaterialService
        {
            private readonly SigningCredentials _signingCredentials;
            private readonly SecurityKeyInfo _validationKey;

            public TestKeyMaterialService()
            {
                var key = new SymmetricSecurityKey(new byte[16]);
                _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                _validationKey = new SecurityKeyInfo
                {
                    Key = key,
                    SigningAlgorithm = SecurityAlgorithms.HmacSha256
                };
            }

            public Task<IEnumerable<SigningCredentials>> GetAllSigningCredentialsAsync()
            {
                return Task.FromResult<IEnumerable<SigningCredentials>>(new[] { _signingCredentials });
            }

            public Task<SigningCredentials> GetSigningCredentialsAsync(IEnumerable<string> allowedAlgorithms = null)
            {
                return Task.FromResult(_signingCredentials);
            }

            public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
            {
                return Task.FromResult<IEnumerable<SecurityKeyInfo>>(new[] { _validationKey });
            }
        }

        private readonly IKeyMaterialService _service;

        public KeyMaterialServiceTests()
        {
            _service = new TestKeyMaterialService();
        }

        [Fact]
        public async Task GetValidationKeys_Should_Return_Expected_Keys()
        {
            var keys = await _service.GetValidationKeysAsync();
            keys.Should().NotBeNull();
            keys.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetSigningCredentials_Should_Return_Credentials()
        {
            var creds = await _service.GetSigningCredentialsAsync();
            creds.Should().NotBeNull();
            creds.Algorithm.Should().Be(SecurityAlgorithms.HmacSha256);
        }

        [Fact]
        public async Task GetAllSigningCredentials_Should_Return_All_Credentials()
        {
            var creds = await _service.GetAllSigningCredentialsAsync();
            creds.Should().NotBeNull();
            creds.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetSigningCredentials_With_AllowedAlgorithms_Should_Return_Credentials()
        {
            var allowedAlgorithms = new[] { SecurityAlgorithms.HmacSha256 };
            var creds = await _service.GetSigningCredentialsAsync(allowedAlgorithms);
            creds.Should().NotBeNull();
            creds.Algorithm.Should().Be(SecurityAlgorithms.HmacSha256);
        }
    }
}
