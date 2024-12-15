using IdentityServer4.Events;

namespace Tests
{
    public class TestEventService : IEventService
    {
        // Implement the methods from IEventService here
        public Task RaiseAsync(Event evt)
        {
            throw new NotImplementedException();
        }

        public Task RaiseAsync<T>(T eventData) where T : Event
        {
            throw new NotImplementedException();
        }
    }
}
