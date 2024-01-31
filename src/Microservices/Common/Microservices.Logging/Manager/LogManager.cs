namespace Microservices.Logging.Manager
{
    using System;
    using Microservices.Logging.Abstract;

    public static class LogManager
    {
        private static ILoggerManager _logger = new LoggerManager();

        public static ILoggerManager LoggerManager
        {
            get => _logger;
            set => _logger = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
