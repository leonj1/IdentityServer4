using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace // Replace with your actual namespace
{
    public class DeviceFlowStore
    {
        private readonly YourDbContext _context; // Replace with your actual DbContext

        public DeviceFlowStore(YourDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var deviceFlowCodes = await _context.DeviceFlowCodes
                .Where(x => x.UserCode == userCode)
                .ToArrayAsync();

            return deviceFlowCodes.SingleOrDefault();
        }

        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCodes = await _context.DeviceFlowCodes
                .Where(x => x.DeviceCode == deviceCode)
                .ToArrayAsync();

            return deviceFlowCodes.SingleOrDefault();
        }

        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var existing = await _context.DeviceFlowCodes
                .Where(x => x.UserCode == userCode)
                .ToArrayAsync();

            if (existing == null)
            {
                throw new InvalidOperationException("Could not update device code");
            }

            existing.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
            existing.Data = Serializer.Serialize(data);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"exception updating {userCode} user code in database: {ex.Message}");
            }
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCodes = await _context.DeviceFlowCodes
                .Where(x => x.DeviceCode == deviceCode)
                .ToArrayAsync();

            if (deviceFlowCodes != null)
            {
                _context.DeviceFlowCodes.Remove(deviceFlowCodes);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"exception removing {deviceCode} device code from database: {ex.Message}");
                }
            }
        }

        protected DeviceFlowCodes ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new DeviceFlowCodes
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = Serializer.Serialize(model)
            };
        }

        protected DeviceCode ToModel(string entity)
        {
            if (entity == null) return null;

            return Serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
