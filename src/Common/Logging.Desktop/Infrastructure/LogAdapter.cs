
namespace Logging.Desktop.Infrastructure
{
    using log4net;
    using Logging.Desktop.Abstract;

    public sealed class LogAdapter : ILogAdapter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LogAdapter));

        public void Fatal(System.Exception ex, string format, params object[] args)
        {
            Log.Fatal(string.Format(format, args), ex);
        }

        public void Fatal(string format, params object[] args)
        {
            Log.FatalFormat(format, args);
        }

        public void Error(System.Exception ex, string format, params object[] args)
        {
            Log.Error(string.Format(format, args), ex);
        }

        public void Error(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log.WarnFormat(format, args);
        }

        public void Debug(string format, params object[] args)
        {
            Log.DebugFormat(format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }
    }
}
