using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class ScopeExtensionsTests
    {
        [Fact]
        public void ToSpaceSeparatedString_Should_Join_Scope_Names_With_Spaces()
        {
            // Arrange
            var scopes = new List<ApiScope>
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
                new ApiScope("scope3")
            };

            // Act
            var result = scopes.ToSpaceSeparatedString();

            // Assert
            result.Should().Be("scope1 scope2 scope3");
        }

        [Fact]
        public void ToSpaceSeparatedString_Should_Return_Empty_String_For_Empty_List()
        {
            // Arrange
            var scopes = new List<ApiScope>();

            // Act
            var result = scopes.ToSpaceSeparatedString();

            // Assert
            result.Should().Be("");
        }

        [Fact]
        public void ToStringList_Should_Return_List_Of_Scope_Names()
        {
            // Arrange
            var scopes = new List<ApiScope>
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
                new ApiScope("scope3")
            };

            // Act
            var result = scopes.ToStringList();

            // Assert
            result.Should().BeEquivalentTo(new[] { "scope1", "scope2", "scope3" });
        }

        [Fact]
        public void ToStringList_Should_Return_Empty_List_For_Empty_Input()
        {
            // Arrange
            var scopes = new List<ApiScope>();

            // Act
            var result = scopes.ToStringList();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
