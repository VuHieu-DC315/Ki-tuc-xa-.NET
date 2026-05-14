namespace kitucxa.Service
{
    public interface ILoginAttemptService
    {
        Task<bool> IsBlockedAsync(string username, string ipAddress);

        Task<int> RecordFailedAttemptAsync(string username, string ipAddress);

        Task ResetFailedAttemptsAsync(string username, string ipAddress);
    }
}