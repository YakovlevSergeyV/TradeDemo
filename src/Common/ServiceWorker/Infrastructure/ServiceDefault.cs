namespace ServiceWorker.Infrastructure
{
    using System;
    using System.Threading;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Extensions.Logging;
    using ServiceWorker.Abstract;

    public class ServiceDefault : IService
    {
        private Thread _workerThread;
        private readonly IWorker _worker;
        private readonly ILogger<ServiceDefault> _logger;

        public ServiceDefault(
            IWorker worker,
            ILogger<ServiceDefault> logger)
        {
            Guard.Argument(() => worker, Is.NotNull);
            Guard.Argument(() => logger, Is.NotNull);

            _worker = worker;
            _logger = logger;
        }

        public void Start()
        {
            try
            {
                _workerThread = new Thread(_worker.Start);
                _workerThread.Start();
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "Ошибка");
            }
        }

        public void Stop()
        {
            try
            {
                _worker.Stop();
                if (_workerThread != null)
                {
                    _workerThread.Abort();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
