using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.Stores
{
    public class InMemoryDeviceFlowStore
    {
        private Dictionary<string, DeviceCode> _store = new Dictionary<string, DeviceCode>();

        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            _store[userCode] = data;
        }

        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            if (_store.ContainsKey(userCode))
            {
                return _store[userCode];
            }
            return null;
        }

        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            foreach (var item in _store.Values)
            {
                if (item.DeviceCode == deviceCode)
                {
                    return item;
                }
            }
            return null;
        }

        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            if (_store.ContainsKey(userCode))
            {
                _store[userCode] = data;
            }
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            foreach (var key in _store.Keys)
            {
                if (_store[key].DeviceCode == deviceCode)
                {
                    _store.Remove(key);
                    break;
                }
            }
        }
    }
}
