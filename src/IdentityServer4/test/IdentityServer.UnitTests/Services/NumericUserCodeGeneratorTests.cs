using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class NumericUserCodeGeneratorTests
    {
        private readonly NumericUserCodeGenerator _target;

        public NumericUserCodeGeneratorTests()
        {
            _target = new NumericUserCodeGenerator();
        }

        [Fact]
        public void UserCodeType_Should_Return_Numeric()
        {
            _target.UserCodeType.Should().Be(IdentityServerConstants.UserCodeTypes.Numeric);
        }

        [Fact]
        public void RetryLimit_Should_Return_Expected_Value()
        {
            _target.RetryLimit.Should().Be(5);
        }

        [Fact]
        public async Task GenerateAsync_Should_Return_9_Digit_Number()
        {
            var result = await _target.GenerateAsync();

            result.Length.Should().Be(9);
            Int32.TryParse(result, out _).Should().BeTrue();
        }

        [Fact]
        public async Task GenerateAsync_Should_Return_Number_Within_Range()
        {
            var result = await _target.GenerateAsync();
            
            var number = Int32.Parse(result);
            number.Should().BeGreaterOrEqualTo(100000000);
            number.Should().BeLessOrEqualTo(999999999);
        }

        [Fact]
        public async Task GenerateAsync_Should_Return_Different_Values()
        {
            var results = new HashSet<string>();
            
            for (int i = 0; i < 10; i++)
            {
                var result = await _target.GenerateAsync();
                results.Add(result).Should().BeTrue("Generated codes should be unique");
            }

            results.Count.Should().Be(10);
        }
    }
}
