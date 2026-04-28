using StackExchange.Redis;

namespace kitucxa.Service
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly IConnectionMultiplexer _redis;

        private const int MaxFailedAttempts = 5;

        private static readonly TimeSpan FailedAttemptWindow = TimeSpan.FromMinutes(5);

        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(5);

        public LoginAttemptService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> IsBlockedAsync(string username, string ipAddress)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return false;
                }

                var db = _redis.GetDatabase();

                string blockKey = GetBlockKey(username, ipAddress);

                bool isBlocked = await db.KeyExistsAsync(blockKey);

                return isBlocked;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> RecordFailedAttemptAsync(string username, string ipAddress)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return 0;
                }

                var db = _redis.GetDatabase();

                string failKey = GetFailKey(username, ipAddress);

                long failedAttempts = await db.StringIncrementAsync(failKey);

                if (failedAttempts == 1)
                {
                    await db.KeyExpireAsync(failKey, FailedAttemptWindow);
                }

                if (failedAttempts >= MaxFailedAttempts)
                {
                    string blockKey = GetBlockKey(username, ipAddress);

                    await db.StringSetAsync(blockKey, "blocked", BlockDuration);

                    await db.KeyDeleteAsync(failKey);
                }

                return (int)failedAttempts;
            }
            catch
            {
                return 0;
            }
        }

        public async Task ResetFailedAttemptsAsync(string username, string ipAddress)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return;
                }

                var db = _redis.GetDatabase();

                string failKey = GetFailKey(username, ipAddress);
                string blockKey = GetBlockKey(username, ipAddress);

                await db.KeyDeleteAsync(failKey);
                await db.KeyDeleteAsync(blockKey);
            }
            catch
            {
                return;
            }
        }

        private string GetFailKey(string username, string ipAddress)
        {
            return $"login:fail:{username}:{ipAddress}";
        }

        private string GetBlockKey(string username, string ipAddress)
        {
            return $"login:block:{username}:{ipAddress}";
        }
    }
}