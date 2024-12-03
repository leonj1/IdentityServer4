using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToSpaceSeparatedString_should_handle_null_input()
        {
            List<string> list = null;
            var result = list.ToSpaceSeparatedString();
            result.Should().Be("");
        }

        [Fact]
        public void ToSpaceSeparatedString_should_handle_empty_list()
        {
            var list = new List<string>();
            var result = list.ToSpaceSeparatedString();
            result.Should().Be("");
        }

        [Fact]
        public void ToSpaceSeparatedString_should_handle_single_item()
        {
            var list = new List<string> { "one" };
            var result = list.ToSpaceSeparatedString();
            result.Should().Be("one");
        }

        [Fact]
        public void ToSpaceSeparatedString_should_handle_multiple_items()
        {
            var list = new List<string> { "one", "two", "three" };
            var result = list.ToSpaceSeparatedString();
            result.Should().Be("one two three");
        }

        [Fact]
        public void FromSpaceSeparatedString_should_handle_null_input()
        {
            var result = "".FromSpaceSeparatedString();
            result.Should().BeEmpty();
        }

        [Fact]
        public void FromSpaceSeparatedString_should_handle_empty_input()
        {
            var result = "".FromSpaceSeparatedString();
            result.Should().BeEmpty();
        }

        [Fact]
        public void FromSpaceSeparatedString_should_handle_single_item()
        {
            var result = "one".FromSpaceSeparatedString();
            result.Should().ContainSingle().And.Contain("one");
        }

        [Fact]
        public void FromSpaceSeparatedString_should_handle_multiple_items()
        {
            var result = "one two three".FromSpaceSeparatedString();
            result.Should().HaveCount(3)
                .And.Contain("one")
                .And.Contain("two")
                .And.Contain("three");
        }

        [Fact]
        public void IsPresent_should_handle_null_input()
        {
            string value = null;
            value.IsPresent().Should().BeFalse();
        }

        [Fact]
        public void IsPresent_should_handle_empty_input()
        {
            "".IsPresent().Should().BeFalse();
        }

        [Fact]
        public void IsPresent_should_handle_whitespace()
        {
            "   ".IsPresent().Should().BeFalse();
        }

        [Fact]
        public void IsPresent_should_handle_valid_input()
        {
            "value".IsPresent().Should().BeTrue();
        }

        [Fact]
        public void EnsureTrailingSlash_should_add_slash_if_missing()
        {
            var url = "http://localhost";
            url.EnsureTrailingSlash().Should().Be("http://localhost/");
        }

        [Fact]
        public void EnsureTrailingSlash_should_not_add_slash_if_present()
        {
            var url = "http://localhost/";
            url.EnsureTrailingSlash().Should().Be("http://localhost/");
        }

        [Fact]
        public void RemoveTrailingSlash_should_remove_slash()
        {
            var url = "http://localhost/";
            url.RemoveTrailingSlash().Should().Be("http://localhost");
        }

        [Fact]
        public void RemoveTrailingSlash_should_handle_no_slash()
        {
            var url = "http://localhost";
            url.RemoveTrailingSlash().Should().Be("http://localhost");
        }

        [Theory]
        [InlineData("http://localhost", false)]
        [InlineData("/path", true)]
        [InlineData("~/path", true)]
        [InlineData("//path", false)]
        [InlineData(@"/\path", false)]
        public void IsLocalUrl_should_properly_validate_urls(string url, bool expected)
        {
            url.IsLocalUrl().Should().Be(expected);
        }

        [Fact]
        public void GetOrigin_should_return_null_for_invalid_url()
        {
            "invalid".GetOrigin().Should().BeNull();
        }

        [Fact]
        public void GetOrigin_should_return_origin_for_valid_url()
        {
            "https://localhost:5000/path".GetOrigin().Should().Be("https://localhost:5000");
        }

        [Theory]
        [InlineData("password123", "****123")]
        [InlineData("123", "****123")]
        [InlineData("12", "****12")]
        [InlineData("", "********")]
        public void Obfuscate_should_properly_hide_value(string input, string expected)
        {
            input.Obfuscate().Should().Be(expected);
        }
    }
}
