using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class IntrospectionResponseGeneratorTests
    {
        private class TestIntrospectionResponseGenerator : IIntrospectionResponseGenerator
        {
            public async Task<Dictionary<string, object>> ProcessAsync(IntrospectionRequestValidationResult validationResult)
            {
                if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
                
                return new Dictionary<string, object>
                {
                    { "active", validationResult.IsActive }
                };
            }
        }

        [Fact]
        public async Task ProcessAsync_WhenValidationResultIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var generator = new TestIntrospectionResponseGenerator();

            // Act
            Func<Task> act = async () => await generator.ProcessAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("validationResult");
        }

        [Fact]
        public async Task ProcessAsync_WhenValidationResultIsValid_ShouldReturnDictionaryWithActiveStatus()
        {
            // Arrange
            var generator = new TestIntrospectionResponseGenerator();
            var validationResult = new IntrospectionRequestValidationResult
            {
                IsActive = true
            };

            // Act
            var result = await generator.ProcessAsync(validationResult);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("active");
            result["active"].Should().Be(true);
        }
    }
}
