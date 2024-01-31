namespace Microservices.Logging.Manager
{
    using Microservices.Logging.Abstract;
    using Microsoft.Extensions.Logging;

    public class LoggerManager : ILoggerManager
    {
        public LoggerManager()
        {
            LoggerLevel = LogLevel.Error;
        }

        public LogLevel LoggerLevel { get; set; }
    }
}
