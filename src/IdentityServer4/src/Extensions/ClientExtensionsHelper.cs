// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.Models
{
    /// <summary>
    /// Helper methods for client extensions.
    /// </summary>
    public static class ClientExtensionsHelper
    {
        /// <summary>
        /// Returns true if the client is an implicit-only client.
        /// </summary>
        public static bool IsImplicitOnly(this Client client)
        {
            return client != null &&
                client.AllowedGrantTypes != null &&
                client.AllowedGrantTypes.Count == 1 &&
                client.AllowedGrantTypes.First() == GrantType.Implicit;
        }
    }
}
