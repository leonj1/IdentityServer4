using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class NumericUserCodeGeneratorTests
    {
        private readonly NumericUserCodeGenerator _sut;

        public NumericUserCodeGeneratorTests()
        {
            _sut = new NumericUserCodeGenerator();
        }

        [Fact]
        public async Task GenerateAsync_should_return_expected_code()
        {
            var userCode = await _sut.GenerateAsync();
            var userCodeInt = int.Parse(userCode);

            userCodeInt.Should().BeGreaterOrEqualTo(100000000);
            userCodeInt.Should().BeLessOrEqualTo(999999999);
        }

        [Fact]
        public async Task GenerateAsync_should_return_nine_digit_code()
        {
            var userCode = await _sut.GenerateAsync();
            userCode.Length.Should().Be(9);
        }

        [Fact]
        public async Task GenerateAsync_should_return_only_numeric_characters()
        {
            var userCode = await _sut.GenerateAsync();
            userCode.Should().MatchRegex("^[0-9]+$");
        }

        [Fact]
        public async Task GenerateAsync_should_generate_unique_codes()
        {
            var generatedCodes = new HashSet<string>();
            
            // Generate multiple codes
            for (int i = 0; i < 100; i++)
            {
                var code = await _sut.GenerateAsync();
                generatedCodes.Add(code).Should().BeTrue("Generated codes should be unique");
            }

            generatedCodes.Count.Should().Be(100);
        }
    }
}
