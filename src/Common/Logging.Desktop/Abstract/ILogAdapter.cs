
namespace Logging.Desktop.Abstract
{
    using System;

    public interface ILogAdapter
    {
        void Fatal(Exception ex, string format, params object[] args);

        void Fatal(string format, params object[] args);

        void Error(Exception ex, string format, params object[] args);

        void Error(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Debug(string format, params object[] args);

        void Info(string format, params object[] args);
    }
}
