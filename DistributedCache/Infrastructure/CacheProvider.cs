using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace DistributedCache.Infrastructure
{
    
    public interface ICacheProvider
    {
        Task<T> GetFromCache<T>(string key) where T : class;
        Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        void ClearCache(string key);
    }
    
    public class CacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cache;

        public CacheProvider(IDistributedCache cache)
        {
            _cache = cache;
        }
        
        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, response , options);
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }
    }
}