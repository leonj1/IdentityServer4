using System;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Validation.AuthorizeRequest_Validation
{
    public class StubAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        public Action<CustomAuthorizeRequestValidationContext> Callback;
        public bool WasCalled { get; set; }

        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            WasCalled = true;
            Callback?.Invoke(context);
            return Task.CompletedTask;
        }
    }
}
