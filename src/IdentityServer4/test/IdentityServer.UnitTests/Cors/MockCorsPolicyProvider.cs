using System;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace IdentityServer.UnitTests.Cors
{
    public class MockCorsPolicyProvider : ICorsPolicyProvider
    {
        public bool WasCalled { get; private set; }

        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            WasCalled = true;
            return Task.FromResult(new CorsPolicy());
        }
    }
}
