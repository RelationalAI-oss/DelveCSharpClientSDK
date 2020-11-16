using System;

namespace DelveClientSDK.Helpers
{
    public interface ILogger
    {
        void Info(string msg);

        void Warning(string msg);

        void Error(string msg, Exception exception = null);
    }
}
