using System.Collections.Specialized;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using IdentityServer4.Extensions;

namespace IdentityServer4.UnitTests.Extensions
{
    public class NameValueCollectionExtensionsTests
    {
        [Fact]
        public void ToFullDictionary_Should_Convert_NameValueCollection_Correctly()
        {
            // Arrange
            var nvc = new NameValueCollection();
            nvc.Add("key1", "value1");
            nvc.Add("key1", "value2");
            nvc.Add("key2", "value3");

            // Act
            var result = nvc.ToFullDictionary();

            // Assert
            result.Should().ContainKey("key1").WhoseValue.Should().BeEquivalentTo(new[] { "value1", "value2" });
            result.Should().ContainKey("key2").WhoseValue.Should().BeEquivalentTo(new[] { "value3" });
        }

        [Fact]
        public void FromFullDictionary_Should_Convert_Dictionary_Correctly()
        {
            // Arrange
            var dict = new Dictionary<string, string[]>
            {
                { "key1", new[] { "value1", "value2" } },
                { "key2", new[] { "value3" } }
            };

            // Act
            var result = dict.FromFullDictionary();

            // Assert
            result.GetValues("key1").Should().BeEquivalentTo(new[] { "value1", "value2" });
            result.GetValues("key2").Should().BeEquivalentTo(new[] { "value3" });
        }

        [Fact]
        public void ToQueryString_Should_Create_Valid_QueryString()
        {
            // Arrange
            var nvc = new NameValueCollection();
            nvc.Add("key1", "value1");
            nvc.Add("key2", "value 2");
            nvc.Add("key3", string.Empty);

            // Act
            var result = nvc.ToQueryString();

            // Assert
            result.Should().Be("key1=value1&key2=value%202&key3");
        }

        [Fact]
        public void ToFormPost_Should_Create_Valid_Html_Input_Fields()
        {
            // Arrange
            var nvc = new NameValueCollection();
            nvc.Add("key1", "value1");
            nvc.Add("key2", "<script>alert('xss')</script>");

            // Act
            var result = nvc.ToFormPost();

            // Assert
            result.Should().Contain("<input type='hidden' name='key1' value='value1' />");
            result.Should().Contain("<input type='hidden' name='key2' value='&lt;script&gt;alert(&#x27;xss&#x27;)&lt;/script&gt;' />");
        }

        [Fact]
        public void ToDictionary_Should_Convert_NameValueCollection_To_Dictionary()
        {
            // Arrange
            var nvc = new NameValueCollection();
            nvc.Add("key1", "value1");
            nvc.Add("key2", "value2");

            // Act
            var result = nvc.ToDictionary();

            // Assert
            result.Should().ContainKey("key1").WhoseValue.Should().Be("value1");
            result.Should().ContainKey("key2").WhoseValue.Should().Be("value2");
        }

        [Fact]
        public void ToScrubbedDictionary_Should_Redact_Sensitive_Values()
        {
            // Arrange
            var nvc = new NameValueCollection();
            nvc.Add("username", "john");
            nvc.Add("password", "secret");
            nvc.Add("client_secret", "confidential");

            // Act
            var result = nvc.ToScrubbedDictionary("password", "client_secret");

            // Assert
            result["username"].Should().Be("john");
            result["password"].Should().Be("***REDACTED***");
            result["client_secret"].Should().Be("***REDACTED***");
        }
    }
}
