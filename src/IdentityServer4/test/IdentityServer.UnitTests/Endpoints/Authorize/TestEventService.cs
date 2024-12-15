using IdentityServer4.Events;

namespace IdentityServer.UnitTests.Common
{
    public class TestEventService : IEventService
    {
        public Task RaiseAsync(Event evt)
        {
            // Implementation of RaiseAsync method
            return Task.CompletedTask;
        }
    }
}
