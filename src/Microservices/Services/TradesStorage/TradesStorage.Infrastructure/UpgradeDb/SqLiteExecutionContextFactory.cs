namespace TradesStorage.Infrastructure.UpgradeDb
{
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Extensions.Logging;

    public class SqLiteExecutionContextFactory : IExecutionContextFactory
    {
        private readonly IOptionsSqLite _options;
        private readonly ILogger<SqLiteExecutionContext> _logger;

        public SqLiteExecutionContextFactory(
            IOptionsSqLite options
            , ILogger<SqLiteExecutionContext> logger)
        {
            Guard.Argument(() => options, Is.NotNull);
            Guard.Argument(() => logger, Is.NotNull);

            _options = options;
            _logger = logger;
        }

        public IExecutionContext Create()
        {
            return new SqLiteExecutionContext(_options, _logger);
        }
    }
}