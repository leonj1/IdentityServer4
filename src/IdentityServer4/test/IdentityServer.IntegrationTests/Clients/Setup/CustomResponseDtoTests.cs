using Xunit;
using FluentAssertions;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class CustomResponseDtoTests
    {
        [Fact]
        public void Create_Should_Return_Valid_CustomResponseDto()
        {
            // Act
            var result = CustomResponseDto.Create;

            // Assert
            result.Should().NotBeNull();
            result.string_value.Should().Be("dto_string");
            result.int_value.Should().Be(43);
            
            result.nested.Should().NotBeNull();
            result.nested.string_value.Should().Be("dto_nested_string");
            result.nested.int_value.Should().Be(44);
        }

        [Fact]
        public void CustomResponseDto_Properties_Should_Be_Settable()
        {
            // Arrange
            var dto = new CustomResponseDto
            {
                string_value = "test_string",
                int_value = 123,
                nested = new CustomResponseDto
                {
                    string_value = "nested_test",
                    int_value = 456
                }
            };

            // Assert
            dto.string_value.Should().Be("test_string");
            dto.int_value.Should().Be(123);
            dto.nested.string_value.Should().Be("nested_test");
            dto.nested.int_value.Should().Be(456);
        }
    }
}
