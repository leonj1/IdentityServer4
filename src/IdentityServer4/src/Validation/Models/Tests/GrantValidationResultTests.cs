using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class GrantValidationResultTests
    {
        [Fact]
        public void Constructor_WithNoSubject_ShouldNotBeError()
        {
            var result = new GrantValidationResult();
            
            Assert.False(result.IsError);
            Assert.Null(result.Subject);
            Assert.Empty(result.CustomResponse);
        }

        // Other test methods remain here...
    }
}
