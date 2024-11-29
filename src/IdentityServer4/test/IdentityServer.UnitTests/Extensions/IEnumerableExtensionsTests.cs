using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_Should_Return_True_For_Null_List()
        {
            IEnumerable<string> list = null;
            list.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_True_For_Empty_List()
        {
            var list = Enumerable.Empty<string>();
            list.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_False_For_Populated_List()
        {
            var list = new[] { "item1", "item2" };
            list.IsNullOrEmpty().Should().BeFalse();
        }

        [Fact]
        public void HasDuplicates_Should_Return_True_When_Duplicates_Exist()
        {
            var list = new[] { "item1", "item2", "item1" };
            list.HasDuplicates(x => x).Should().BeTrue();
        }

        [Fact]
        public void HasDuplicates_Should_Return_False_When_No_Duplicates_Exist()
        {
            var list = new[] { "item1", "item2", "item3" };
            list.HasDuplicates(x => x).Should().BeFalse();
        }

        [Fact]
        public void HasDuplicates_Should_Work_With_Complex_Types()
        {
            var list = new[]
            {
                new { Id = 1, Name = "First" },
                new { Id = 2, Name = "Second" },
                new { Id = 1, Name = "Third" }
            };

            list.HasDuplicates(x => x.Id).Should().BeTrue();
            list.HasDuplicates(x => x.Name).Should().BeFalse();
        }
    }
}
