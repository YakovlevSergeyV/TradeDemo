
namespace Logging.Desktop.Infrastructure
{
    using log4net;
    using NLog;
    using NLog.Targets;

    [Target("NLogToLog4NetAdapter")]
    public sealed class NLogToLog4NetAdapter : TargetWithLayout
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(LogAdapter));

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Level == LogLevel.Error)
            {
                Log.Error(logEvent.FormattedMessage, logEvent.Exception);
            }

            if (logEvent.Level == LogLevel.Fatal)
            {
                Log.Fatal(logEvent.FormattedMessage, logEvent.Exception);
            }

            if (logEvent.Level == LogLevel.Info)
            {
                Log.Info(logEvent.FormattedMessage);
            }

            if (logEvent.Level == LogLevel.Debug)
            {
                Log.Debug(logEvent.FormattedMessage);
            }

            if (logEvent.Level == LogLevel.Warn)
            {
                Log.Warn(logEvent.FormattedMessage);
            }
        }
    }
}
