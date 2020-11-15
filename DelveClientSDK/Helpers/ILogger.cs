using System;

namespace DelveClientSDK.Helpers
{
    public interface ILogger
    {
        void LogInformation(string msg);

        void LogWarning(string msg);

        void LogError(string msg, Exception exception = null);
    }
}
