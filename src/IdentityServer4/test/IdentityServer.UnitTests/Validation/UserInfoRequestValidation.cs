// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.UnitTests.Common;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class UserInfoRequestValidation
    {
        private const string Category = "UserInfo Request Validation Tests";
        private IClientStore _clients = new InMemoryClientStore(TestClients.Get());

        [Fact]
        [Trait("Category", Category)]
        public async Task token_without_sub_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>()
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task active_user_should_succeed()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>
                {
                    new Claim("sub", "123")
                },
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task inactive_user_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>
                {
                    new Claim("sub", "123")
                },
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: false),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task invalid_token_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = true,
                Error = "invalid_token",
                Client = await _clients.FindEnabledClientByIdAsync("codeclient")
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task missing_token_should_fail()
        {
            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(new TokenValidationResult()),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync(null);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task multiple_sub_claims_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>
                {
                    new Claim("sub", "123"),
                    new Claim("sub", "456")
                },
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }
    }
}
