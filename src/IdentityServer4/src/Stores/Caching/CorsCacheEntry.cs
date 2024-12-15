using IdentityServer4.Extensions;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Class to model entries in CORS origin cache.
    /// </summary>
    public class CorsCacheEntry
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CorsCacheEntry(bool allowed)
        {
            Allowed = allowed;
        }

        /// <summary>
        /// Is origin allowed.
        /// </summary>
        public bool Allowed { get; }
    }
}
