using System;
using System.Collections.Generic;
using Xunit;
using IdentityServer4.Extensions;

namespace IdentityServer4.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", true)]
        [InlineData(" ", true)]
        [InlineData(null, true)]
        [InlineData("abc", false)]
        public void IsMissing_Should_Detect_Missing_String(string input, bool expected)
        {
            var result = input.IsMissing();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        [InlineData("abc", true)]
        public void IsPresent_Should_Detect_Present_String(string input, bool expected)
        {
            var result = input.IsPresent();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(new[] { "a", "b", "c" }, "a b c")]
        [InlineData(new string[] { }, "")]
        [InlineData(null, "")]
        public void ToSpaceSeparatedString_Should_Join_Strings(string[] input, string expected)
        {
            var result = input.ToSpaceSeparatedString();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("a b c", new[] { "a", "b", "c" })]
        [InlineData("", new string[] { })]
        [InlineData(" ", new string[] { })]
        public void FromSpaceSeparatedString_Should_Split_String(string input, string[] expected)
        {
            var result = input.FromSpaceSeparatedString();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("test", "/test")]
        [InlineData("/test", "/test")]
        [InlineData(null, null)]
        public void EnsureLeadingSlash_Should_Add_Slash(string input, string expected)
        {
            var result = input.EnsureLeadingSlash();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("test", "test/")]
        [InlineData("test/", "test/")]
        [InlineData(null, null)]
        public void EnsureTrailingSlash_Should_Add_Slash(string input, string expected)
        {
            var result = input.EnsureTrailingSlash();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1234567890", "****7890")]
        [InlineData("123", "****123")]
        [InlineData("", "****")]
        [InlineData(null, "****")]
        public void Obfuscate_Should_Mask_String(string input, string expected)
        {
            var result = input.Obfuscate();
            Assert.Equal(expected, result);
        }
    }
}
