using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockKeyMaterialServiceTests
    {
        [Fact]
        public async Task GetAllSigningCredentials_Should_Return_All_Credentials()
        {
            // Arrange
            var service = new MockKeyMaterialService();
            var cred1 = new SigningCredentials(new SymmetricSecurityKey(new byte[16]), "RS256");
            var cred2 = new SigningCredentials(new SymmetricSecurityKey(new byte[16]), "RS384");
            service.SigningCredentials.Add(cred1);
            service.SigningCredentials.Add(cred2);

            // Act
            var result = await service.GetAllSigningCredentialsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(cred1);
            result.Should().Contain(cred2);
        }

        [Fact]
        public async Task GetSigningCredentials_Should_Return_First_Credential()
        {
            // Arrange
            var service = new MockKeyMaterialService();
            var cred1 = new SigningCredentials(new SymmetricSecurityKey(new byte[16]), "RS256");
            var cred2 = new SigningCredentials(new SymmetricSecurityKey(new byte[16]), "RS384");
            service.SigningCredentials.Add(cred1);
            service.SigningCredentials.Add(cred2);

            // Act
            var result = await service.GetSigningCredentialsAsync();

            // Assert
            result.Should().Be(cred1);
        }

        [Fact]
        public async Task GetValidationKeys_Should_Return_All_Keys()
        {
            // Arrange
            var service = new MockKeyMaterialService();
            var key1 = new SecurityKeyInfo { Key = new SymmetricSecurityKey(new byte[16]) };
            var key2 = new SecurityKeyInfo { Key = new SymmetricSecurityKey(new byte[16]) };
            service.ValidationKeys.Add(key1);
            service.ValidationKeys.Add(key2);

            // Act
            var result = await service.GetValidationKeysAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(key1);
            result.Should().Contain(key2);
        }
    }
}
