
namespace Logging.Desktop.Infrastructure
{
    using CommandLine;

    public sealed class Log4netSettings
    {
        [Option('b', "bd", Required = true)]
        public string BaseDirectory { get; set; }

        [Option('n', "l4n", Required = true)]
        public string Log4netConfigFileName { get; set; }

        [Option('f', "l4nFn", Required = true)]
        public string Log4netFileName { get; set; }
    }
}
