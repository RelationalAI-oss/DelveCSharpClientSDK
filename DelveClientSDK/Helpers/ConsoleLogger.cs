using System;

namespace DelveClientSDK.Helpers
{
    public class ConsoleLogger : ILogger
    {
        public void LogInformation(string msg)
        {
            Console.WriteLine(msg);
        }

        public void LogWarning(string msg)
        {
            Console.WriteLine($"[WARN] {msg}");
        }

        public void LogError(string msg, Exception exception = null)
        {
            Console.WriteLine($"[ERROR] {msg}\n{exception?.Message}\n{exception?.StackTrace}");
        }
    }
}
