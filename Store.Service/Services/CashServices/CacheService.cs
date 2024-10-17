using StackExchange.Redis;
using Store.Service.Services.CachServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Service.Services.CacheServices
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _database;
        public CacheService(IConnectionMultiplexer redis)
        {
            _database=redis.GetDatabase();
        }
        public async Task<string> GetCacheResponseAsync(string key)
        {
            var casheResponse = await _database.StringGetAsync(key);
            if (casheResponse.IsNullOrEmpty) return null;

            return casheResponse.ToString();
        }

        public async Task SetCacheResponseAsync(string key, object response, TimeSpan timeToLive)
        {
            if (response is null)
                return;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper };

            var serializedResponse = JsonSerializer.Serialize(response, options);
            await _database.StringSetAsync(key, serializedResponse, timeToLive);

        }
    }
}
