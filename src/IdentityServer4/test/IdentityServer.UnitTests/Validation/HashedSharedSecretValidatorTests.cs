using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class HashedSharedSecretValidatorTests
    {
        private readonly HashedSharedSecretValidator _validator;
        private readonly List<Secret> _secrets;

        public HashedSharedSecretValidatorTests()
        {
            _validator = new HashedSharedSecretValidator(TestLogger.Create<HashedSharedSecretValidator>());
            _secrets = new List<Secret>();
        }

        [Fact]
        public async Task Valid_SHA256_Secret_Should_Succeed()
        {
            var secret = "secret";
            var sha256 = secret.Sha256();
            
            _secrets.Add(new Secret { Value = sha256 });
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Credential = secret,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(_secrets, parsedSecret);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Valid_SHA512_Secret_Should_Succeed()
        {
            var secret = "secret";
            var sha512 = secret.Sha512();
            
            _secrets.Add(new Secret { Value = sha512 });
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Credential = secret,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(_secrets, parsedSecret);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Invalid_Secret_Type_Should_Fail()
        {
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Credential = "secret",
                Type = "invalid_type"
            };

            var result = await _validator.ValidateAsync(_secrets, parsedSecret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Missing_Secret_Should_Throw()
        {
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            await Assert.ThrowsAsync<ArgumentException>(() => 
                _validator.ValidateAsync(_secrets, parsedSecret));
        }

        [Fact]
        public async Task Invalid_Hash_Length_Should_Fail()
        {
            _secrets.Add(new Secret { Value = "invalid_length_hash" });
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(_secrets, parsedSecret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Non_Matching_Secret_Should_Fail()
        {
            var sha256 = "different_secret".Sha256();
            _secrets.Add(new Secret { Value = sha256 });
            
            var parsedSecret = new ParsedSecret
            {
                Id = "id",
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(_secrets, parsedSecret);
            result.Success.Should().BeFalse();
        }
    }
}
