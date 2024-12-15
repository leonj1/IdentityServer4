using System;
using System.Threading.Tasks;

public class DefaultGrantStore : IGrantStore
{
    private readonly IPersistedGrantStore _store;
    private readonly ISerializer _serializer;
    private readonly ILogger _logger;
    private readonly IHandleGenerationService _handleGenerationService;

    public DefaultGrantStore(IPersistedGrantStore store, ISerializer serializer, ILogger logger, IHandleGenerationService handleGenerationService)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handleGenerationService = handleGenerationService ?? throw new ArgumentNullException(nameof(handleGenerationService));
    }

    public string GrantType { get; set; } = "default";

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    protected virtual async Task<T> GetItemAsync<T>(string key)
    {
        var hashedKey = GetHashedKey(key);

        var grant = await _store.GetAsync(hashedKey);
        if (grant != null && grant.Type == GrantType)
        {
            try
            {
                return _serializer.Deserialize<T>(grant.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON from grant store.");
            }
        }
        else
        {
            _logger.LogWarning("Grant not found or type mismatch.");
        }

        return default;
    }

    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    protected virtual async Task RemoveItemAsync(string key)
    {
        key = GetHashedKey(key);
        await _store.RemoveAsync(key);
    }

    /// <summary>
    /// Removes all items for a subject id / cliend id combination.
    /// </summary>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <returns></returns>
    protected virtual async Task RemoveAllAsync(string subjectId, string clientId)
    {
        await _store.RemoveAllAsync(new PersistedGrantFilter
        {
            SubjectId = subjectId,
            ClientId = clientId,
            Type = GrantType
        });
    }

    private string GetHashedKey(string key)
    {
        // Implement hashing logic here
        return key;
    }
}
