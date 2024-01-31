namespace Microservices.Logging.Abstract
{
    using Microsoft.Extensions.Logging;

    public interface ILoggerManager
    {
        LogLevel LoggerLevel { get; set;  }
    }
}
