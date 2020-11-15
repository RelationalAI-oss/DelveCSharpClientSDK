using System;

namespace DelveClientSDK.Helpers
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Warning(string msg)
        {
            Console.WriteLine($"[WARN] {msg}");
        }

        public void Error(string msg, Exception exception = null)
        {
            Console.WriteLine($"[ERROR] {msg}\n{exception?.Message}\n{exception?.StackTrace}");
        }
    }
}
