using Microsoft.AspNetCore.SignalR.Client;

namespace BackendClient;

public class RetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        var previousRetryCount = retryContext.PreviousRetryCount;
        return TimeSpan.FromSeconds(previousRetryCount);
    }
}
