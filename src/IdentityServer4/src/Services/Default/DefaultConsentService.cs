using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourNamespace // Replace with your actual namespace
{
    public class DefaultConsentService
    {
        private readonly IUserConsentStore _userConsentStore;
        private readonly IClientStore _clientStore;
        private readonly IClock _clock;

        public DefaultConsentService(IUserConsentStore userConsentStore, IClientStore clientStore, IClock clock)
        {
            _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public virtual async Task<bool> RequiresConsentAsync(ClaimsPrincipal subject, Client client, IEnumerable<ParsedScopeValue> parsedScopes)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (parsedScopes == null) throw new ArgumentNullException(nameof(parsedScopes));

            if (!client.AllowRememberConsent)
            {
                return true;
            }

            if (parsedScopes.Any(x => x.ParsedName != x.RawValue))
            {
                return true;
            }

            var scopes = parsedScopes.Select(x => x.RawValue).ToArray();

            if (scopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess))
            {
                return true;
            }

            var consent = await _userConsentStore.GetUserConsentAsync(subject.GetSubjectId(), client.ClientId);

            if (consent == null)
            {
                return true;
            }

            if (consent.Expiration.HasExpired(_clock.UtcNow.UtcDateTime))
            {
                await _userConsentStore.RemoveUserConsentAsync(consent.SubjectId, consent.ClientId);
                return true;
            }

            if (consent.Scopes != null)
            {
                var intersect = scopes.Intersect(consent.Scopes);
                var different = scopes.Count() != intersect.Count();

                return different;
            }

            return true;
        }

        public virtual async Task UpdateConsentAsync(ClaimsPrincipal subject, Client client, IEnumerable<ParsedScopeValue> parsedScopes)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (client.AllowRememberConsent)
            {
                var subjectId = subject.GetSubjectId();
                var clientId = client.ClientId;

                var scopes = parsedScopes?.Select(x => x.RawValue).ToArray();
                if (scopes != null && scopes.Any())
                {
                    var consent = new Consent
                    {
                        CreationTime = _clock.UtcNow.UtcDateTime,
                        SubjectId = subjectId,
                        ClientId = clientId,
                        Scopes = scopes
                    };

                    if (client.ConsentLifetime.HasValue)
                    {
                        consent.Expiration = consent.CreationTime.AddSeconds(client.ConsentLifetime.Value);
                    }

                    await _userConsentStore.StoreUserConsentAsync(consent);
                }
                else
                {
                    await _userConsentStore.RemoveUserConsentAsync(subjectId, clientId);
                }
            }
        }
    }
}
