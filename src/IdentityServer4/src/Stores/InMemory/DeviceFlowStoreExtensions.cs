using System.Threading.Tasks;

namespace IdentityServer4.Stores
{
    public static class DeviceFlowStoreExtensions
    {
        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public static Task UpdateByUserCodeAsync(this IDeviceFlowStore store, string userCode, DeviceCode data)
        {
            lock (store as InMemoryDeviceFlowStore)
            {
                var foundData = ((InMemoryDeviceFlowStore)store)._repository.FirstOrDefault(x => x.UserCode == userCode);

                if (foundData != null)
                {
                    foundData.Data = data;
                }
            }

            return Task.CompletedTask;
        }
    }
}
