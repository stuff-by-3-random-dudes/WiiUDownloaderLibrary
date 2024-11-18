using System;
using Microsoft.Extensions.Logging;

namespace WiiUDownloaderLibrary.Models.DefaultInjectors
{
    public class DefaultLogger : ILogger<Downloader>
    {
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (exception != null)
                message += Environment.NewLine + exception.ToString();

            Console.WriteLine($"[{logLevel}] {message}");
        }
    }
}