// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    /// <summary>
    /// Refactored tests for both null values comparison.
    /// </summary>
    public class BothNullComparison
    {
        [Fact]
        public void id_token_token_both_ways()
        {
            ResponseTypeEqualityComparer comparer = new ResponseTypeEqualityComparer();
            string x = "id_token token";
            string y = "token id_token";
            var result = comparer.Equals(x, y);
            result.Should().BeTrue();
        }
    }
}
