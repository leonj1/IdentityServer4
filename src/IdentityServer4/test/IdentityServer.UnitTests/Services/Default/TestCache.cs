using System;
using System.Collections.Generic;

public class TestCache : IDistributedCache
{
    public readonly Dictionary<string, Tuple<byte[], DistributedCacheEntryOptions>> Items =
        new Dictionary<string, Tuple<byte[], DistributedCacheEntryOptions>>();

    public byte[] Get(string key)
    {
        if (Items.TryGetValue(key, out var value)) return value.Item1;
        return null;
    }

    public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
    {
        return Task.FromResult(Get(key));
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        Items.Remove(key);

        Items.Add(key, new Tuple<byte[], DistributedCacheEntryOptions>(value, options));
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = new CancellationToken())
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }

    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}
