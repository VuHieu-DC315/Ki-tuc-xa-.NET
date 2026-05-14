using System.Text.Json;
using StackExchange.Redis;

namespace kitucxa.Service.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return default;
                }

                var db = _redis.GetDatabase();

                var cachedValue = await db.StringGetAsync(key);

                if (!cachedValue.HasValue)
                {
                    return default;
                }

                var json = cachedValue.ToString();

                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch
            {
                // Nếu Redis lỗi thì bỏ qua, lấy dữ liệu từ database.
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return;
                }

                var db = _redis.GetDatabase();

                var json = JsonSerializer.Serialize(value, JsonOptions);

                await db.StringSetAsync(key, json, expiration);
            }
            catch
            {
                // Không để Redis làm hỏng chức năng chính của website.
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return;
                }

                var db = _redis.GetDatabase();

                await db.KeyDeleteAsync(key);
            }
            catch
            {
                // Không để Redis làm hỏng thao tác thêm/sửa/xóa.
            }
        }
    }
}