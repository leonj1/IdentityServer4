// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;

namespace IdentityServer4.Stores
{
    public static class PersistedGrantStorage
    {
        /// <summary>
        /// Stores a persisted grant asynchronously.
        /// </summary>
        /// <param name="grant">The grant.</param>
        /// <returns></returns>
        public static Task StoreAsync(PersistedGrant grant)
        {
            // Implementation of StoreAsync method
            return Task.CompletedTask;
        }
    }
}
