// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.ResponseHandling
{
    /// <summary>
    /// The userinfo response generator
    /// </summary>
    /// <seealso cref="IdentityServer4.ResponseHandling.IUserInfoResponseGenerator" />
    public class UserInfoResponseGenerator : IUserInfoResponseGenerator
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The profile service
        /// </summary>
        protected readonly IProfileService Profile;

        /// <summary>
        /// The resource store
        /// </summary>
        protected readonly IResourceStore Resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoResponseGenerator"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="logger">The logger.</param>
        public UserInfoResponseGenerator(IProfileService profile, IResourceStore resources, ILogger<UserInfoResponseGenerator> logger)
        {
            Profile = profile;
            Resources = resources;
            Logger = logger;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<IUserClaimsPrincipal> ProcessAsync(UserInfoRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claims = await Profile.GetUserClaims(subjectId, context.RequestedClaimTypes);

            if (claims == null || !claims.Any())
            {
                Logger.LogInformation("Profile service returned no claims (null)");
                return new DefaultUserClaimsPrincipal(new ClaimsIdentity());
            }

            var outgoingClaims = new List<Claim>(claims);
            var subClaim = outgoingClaims.SingleOrDefault(x => x.Type == JwtClaimTypes.Subject);

            if (subClaim == null)
            {
                outgoingClaims.Add(new Claim(JwtClaimTypes.Subject, subjectId));
            }
            else if (subClaim.Value != subjectId)
            {
                Logger.LogError("Profile service returned incorrect subject value: {sub}", subClaim);
                throw new InvalidOperationException("Profile service returned incorrect subject value");
            }

            return new DefaultUserClaimsPrincipal(new ClaimsIdentity(outgoingClaims));
        }
    }
}
