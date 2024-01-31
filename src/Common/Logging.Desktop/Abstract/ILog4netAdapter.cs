
namespace Logging.Desktop.Abstract
{
    using Logging.Desktop.Infrastructure;

    public interface ILog4netAdapter
    {
        void SetFilePathFromCommandLine(Log4netSettings log4netSettings);
    }
}
