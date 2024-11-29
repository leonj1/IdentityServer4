using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class CryptoHelperTests
    {
        [Fact]
        public void CreateRsaSecurityKey_Should_Create_Valid_Key()
        {
            // Act
            var key = CryptoHelper.CreateRsaSecurityKey();

            // Assert
            key.Should().NotBeNull();
            key.KeyId.Should().NotBeNullOrEmpty();
            key.Rsa.KeySize.Should().Be(2048);
        }

        [Fact]
        public void CreateECDsaSecurityKey_Should_Create_Valid_Key()
        {
            // Act
            var key = CryptoHelper.CreateECDsaSecurityKey();

            // Assert
            key.Should().NotBeNull();
            key.KeyId.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("RS256", 32)]
        [InlineData("RS384", 48)]
        [InlineData("RS512", 64)]
        public void CreateHashClaimValue_Should_Create_Valid_Hash(string algorithm, int expectedLength)
        {
            // Arrange
            var value = "test_value";

            // Act
            var hash = CryptoHelper.CreateHashClaimValue(value, algorithm);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            Base64Url.Decode(hash).Length.Should().Be(expectedLength / 2);
        }

        [Theory]
        [InlineData("RS256")]
        [InlineData("RS384")]
        [InlineData("RS512")]
        public void GetHashAlgorithmForSigningAlgorithm_Should_Return_Correct_Algorithm(string algorithm)
        {
            // Act
            var hashAlgorithm = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(algorithm);

            // Assert
            hashAlgorithm.Should().NotBeNull();
            if (algorithm == "RS256")
                hashAlgorithm.Should().BeOfType<SHA256Managed>();
            else if (algorithm == "RS384")
                hashAlgorithm.Should().BeOfType<SHA384Managed>();
            else if (algorithm == "RS512")
                hashAlgorithm.Should().BeOfType<SHA512Managed>();
        }

        [Theory]
        [InlineData("invalid")]
        public void GetHashAlgorithmForSigningAlgorithm_Should_Throw_On_Invalid_Algorithm(string algorithm)
        {
            // Act & Assert
            Action act = () => CryptoHelper.GetHashAlgorithmForSigningAlgorithm(algorithm);
            act.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(JsonWebKeyECTypes.P256)]
        [InlineData(JsonWebKeyECTypes.P384)]
        [InlineData(JsonWebKeyECTypes.P521)]
        public void GetCurveFromCrvValue_Should_Return_Valid_Curve(string crv)
        {
            // Act
            var curve = CryptoHelper.GetCurveFromCrvValue(crv);

            // Assert
            curve.Should().NotBeNull();
        }

        [Fact]
        public void GetCurveFromCrvValue_Should_Throw_On_Invalid_Curve()
        {
            // Act & Assert
            Action act = () => CryptoHelper.GetCurveFromCrvValue("invalid");
            act.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.RS256, SecurityAlgorithms.RsaSha256)]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.RS384, SecurityAlgorithms.RsaSha384)]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.RS512, SecurityAlgorithms.RsaSha512)]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.PS256, SecurityAlgorithms.RsaSsaPssSha256)]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.PS384, SecurityAlgorithms.RsaSsaPssSha384)]
        [InlineData(IdentityServerConstants.RsaSigningAlgorithm.PS512, SecurityAlgorithms.RsaSsaPssSha512)]
        public void GetRsaSigningAlgorithmValue_Should_Return_Correct_Algorithm(
            IdentityServerConstants.RsaSigningAlgorithm input, string expected)
        {
            // Act
            var result = CryptoHelper.GetRsaSigningAlgorithmValue(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(IdentityServerConstants.ECDsaSigningAlgorithm.ES256, SecurityAlgorithms.EcdsaSha256)]
        [InlineData(IdentityServerConstants.ECDsaSigningAlgorithm.ES384, SecurityAlgorithms.EcdsaSha384)]
        [InlineData(IdentityServerConstants.ECDsaSigningAlgorithm.ES512, SecurityAlgorithms.EcdsaSha512)]
        public void GetECDsaSigningAlgorithmValue_Should_Return_Correct_Algorithm(
            IdentityServerConstants.ECDsaSigningAlgorithm input, string expected)
        {
            // Act
            var result = CryptoHelper.GetECDsaSigningAlgorithmValue(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
