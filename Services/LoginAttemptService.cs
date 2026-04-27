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
            var db = _redis.GetDatabase();

            string blockKey = GetBlockKey(username, ipAddress);

            bool isBlocked = await db.KeyExistsAsync(blockKey);

            return isBlocked;
        }

        public async Task<int> RecordFailedAttemptAsync(string username, string ipAddress)
        {
            var db = _redis.GetDatabase();

            string failKey = GetFailKey(username, ipAddress);
            string blockKey = GetBlockKey(username, ipAddress);

            long failedCount = await db.StringIncrementAsync(failKey);

            if (failedCount == 1)
            {
                await db.KeyExpireAsync(failKey, FailedAttemptWindow);
            }

            if (failedCount >= MaxFailedAttempts)
            {
                await db.StringSetAsync(blockKey, "blocked", BlockDuration);

                await db.KeyDeleteAsync(failKey);
            }

            return (int)failedCount;
        }

        public async Task ResetFailedAttemptsAsync(string username, string ipAddress)
        {
            var db = _redis.GetDatabase();

            string failKey = GetFailKey(username, ipAddress);
            string blockKey = GetBlockKey(username, ipAddress);

            await db.KeyDeleteAsync(failKey);
            await db.KeyDeleteAsync(blockKey);
        }

        private string GetFailKey(string username, string ipAddress)
        {
            username = NormalizeUsername(username);
            ipAddress = NormalizeIpAddress(ipAddress);

            return $"login:fail:{username}:{ipAddress}";
        }

        private string GetBlockKey(string username, string ipAddress)
        {
            username = NormalizeUsername(username);
            ipAddress = NormalizeIpAddress(ipAddress);

            return $"login:block:{username}:{ipAddress}";
        }

        private string NormalizeUsername(string username)
        {
            return username.Trim().ToLower();
        }

        private string NormalizeIpAddress(string ipAddress)
        {
            return ipAddress.Trim();
        }
    }
}