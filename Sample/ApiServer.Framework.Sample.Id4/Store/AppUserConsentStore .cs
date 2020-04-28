using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppUserConsentStore : DefaultUserConsentStore
    {
        public AppUserConsentStore(IPersistedGrantStore store, IPersistentGrantSerializer serializer, IHandleGenerationService handleGenerationService, ILogger<DefaultUserConsentStore> logger) : base(store, serializer, handleGenerationService, logger)
        {
        }

        protected override string GetHashedKey(string value)
        {
           return $"{value}:{GrantType}";
        }
    }
}
