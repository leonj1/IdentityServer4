using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultUserCodeServiceTests
    {
        private class TestUserCodeGenerator : IUserCodeGenerator
        {
            public string UserCodeType { get; }

            public TestUserCodeGenerator(string userCodeType)
            {
                UserCodeType = userCodeType;
            }

            public Task<string> GenerateAsync()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task GetGenerator_WhenMatchingGeneratorExists_ShouldReturnCorrectGenerator()
        {
            // Arrange
            var generators = new List<IUserCodeGenerator>
            {
                new TestUserCodeGenerator("type1"),
                new TestUserCodeGenerator("type2"),
                new TestUserCodeGenerator("type3")
            };
            var service = new DefaultUserCodeService(generators);

            // Act
            var result = await service.GetGenerator("type2");

            // Assert
            result.Should().NotBeNull();
            result.UserCodeType.Should().Be("type2");
        }

        [Fact]
        public async Task GetGenerator_WhenNoMatchingGenerator_ShouldReturnNull()
        {
            // Arrange
            var generators = new List<IUserCodeGenerator>
            {
                new TestUserCodeGenerator("type1"),
                new TestUserCodeGenerator("type2")
            };
            var service = new DefaultUserCodeService(generators);

            // Act
            var result = await service.GetGenerator("non-existent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenGeneratorsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new DefaultUserCodeService(null);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("generators");
        }
    }
}
