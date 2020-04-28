using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    
    public class RedisPersistedGrantStore : IPersistedGrantStore
    {

        private string GetKey(string key) => $"{key}";

        private string GetSetKey(string subjectId) => $"{subjectId}";

        private string GetSetKey(string subjectId, string clientId) => $"{subjectId}:{clientId}";

        private string GetSetKey(string subjectId, string clientId, string type) => $"{subjectId}:{clientId}:{type}";


        public Task StoreAsync(PersistedGrant grant)
        {
            
            var expiresIn = (int)((grant.Expiration - System.DateTime.UtcNow).Value.TotalSeconds);

            var grantKey = GetKey(grant.Key);
            var setKey = GetSetKey(grant.SubjectId, grant.ClientId, grant.Type);
            var setKeyforSubject = GetSetKey(grant.SubjectId);
            var setKeyforClient = GetSetKey(grant.SubjectId, grant.ClientId);

            var timeSpan = TimeSpan.FromSeconds(expiresIn);
            if (!string.IsNullOrEmpty(grant.SubjectId))
            {

                RedisHelper.StartPipe()
                .Set(grantKey, grant, expiresIn)
                .HSet(setKeyforSubject, grantKey, null).Expire(setKeyforSubject, expiresIn)
                .HSet(setKeyforClient, grantKey, null).Expire(setKeyforClient, expiresIn)
                .HSet(setKey, grantKey, null).Expire(setKey, expiresIn)
                .EndPipe();
            }
            else {
               RedisHelper.Set(grantKey, grant, expiresIn);
            }
            return Task.CompletedTask;
        }
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var grantKey = GetKey(key);
            var data = await RedisHelper.GetAsync<PersistedGrant>(grantKey);
            return data!=null ? data : null;
        }

        private async Task<(IEnumerable<PersistedGrant> grants, IEnumerable<string> keysToDelete)> GetGrants(string setKey)
        {
            var grantsKeys = await RedisHelper.HGetAllAsync(setKey);
            if (!grantsKeys.Any())
                return (Enumerable.Empty<PersistedGrant>(), Enumerable.Empty<string>());
            var grants = await RedisHelper.MGetAsync<PersistedGrant>(grantsKeys.Select(_ => _.Key).ToArray());

            var keysToDelete = grantsKeys.Zip(grants, (kv, grant) => new KeyValuePair<string, PersistedGrant>(kv.Key, grant))
                .Where(_ => _.Value != null)
                .Select(_ => _.Key);
                
            return (grants.Where(_ => _ != null), keysToDelete);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var setKey = GetSetKey(subjectId);
            var (grants, keysToDelete) = await GetGrants(setKey);
            if (keysToDelete.Any())
                RedisHelper.HDel(setKey, keysToDelete.ToArray());
            return grants;
        }

        public Task RemoveAsync(string key)
        {

            var grantKey = GetKey(key);
            var grant =  RedisHelper.Get<PersistedGrant>(grantKey);
            if (grant == null)
            {
                return Task.CompletedTask;
            }
            RedisHelper.StartPipe()
                .Del(grantKey)
                .HDel(GetSetKey(grant.SubjectId), grantKey)
                .HDel(GetSetKey(grant.SubjectId, grant.ClientId), grantKey)
                .HDel(GetSetKey(grant.SubjectId, grant.ClientId, grant.Type), grantKey)
                .EndPipe();
            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var setKey = GetSetKey(subjectId, clientId);
            var grantsKeysKv = RedisHelper.HGetAll(setKey);
            if (!grantsKeysKv.Any()) {
                return Task.CompletedTask;
            }
            var grantsKeys = grantsKeysKv.Select(_ => _.Key).ToArray();

            var pipe = RedisHelper.StartPipe();
            pipe.Del(setKey);
            foreach (var k in grantsKeys) {
                pipe.Del(k);
            }
            pipe.HDel(GetSetKey(subjectId), grantsKeys);
            pipe.EndPipe();
            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var setKey = GetSetKey(subjectId, clientId, type);
            var grantsKeysKv = RedisHelper.HGetAll(setKey);
            if (!grantsKeysKv.Any()) return Task.CompletedTask;

            var grantsKeys = grantsKeysKv.Select(_ => _.Key).ToArray();
            var pipe = RedisHelper.StartPipe();
            pipe.Del(setKey);
            foreach (var k in grantsKeys)
            {
                pipe.Del(k);
            }
            pipe.HDel(GetSetKey(subjectId, clientId), grantsKeys);
            pipe.HDel(GetSetKey(subjectId), grantsKeys);
            pipe.EndPipe();
            return Task.CompletedTask;
        }
    }
}
