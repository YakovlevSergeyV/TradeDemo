namespace ServiceWorker.Infrastructure
{
    using System;
    using System.Threading;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Extensions.Logging;
    using ServiceWorker.Abstract;

    public class Worker : IWorker
    {
        private const int HeartBeatCycleInMsDefault = 500;

        private DateTime _tickStart;
        private DateTime _startGarbageCollector;
        private bool _isWork;

        private readonly IServiceModelFacade _serviceModelFacade;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IServiceModelFacade serviceModelFacade,
            ILogger<Worker> logger)
        {
            Guard.Argument(() => serviceModelFacade, Is.NotNull);
            Guard.Argument(() => logger, Is.NotNull);

            _serviceModelFacade = serviceModelFacade;
            _logger = logger;

            _isWork = true;
            HeartBeatCycleInMs = HeartBeatCycleInMsDefault;
            _startGarbageCollector = DateTime.UtcNow;
        }

        public int HeartBeatCycleInMs { get; set; }

        /// <summary>
        /// Этот метод - основное тело сервиса
        /// </summary>
        public void Start()
        {
            try
            {
                TryDoWork();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Непредвиденная ошибка основного цикла обработки.");
            }
            finally
            {
                _logger.LogDebug("Выход из сновного основного цикла обработки.");
            }
        }

        public void Stop()
        {
            _isWork = false;
        }

        private void TryDoWork()
        {
            do
            {
                StartTick(); // Запоминаем момент входа в обработчик

                _serviceModelFacade.DoWork();

                Collect();

                EndTick(); // Рапортуем об окончании очередного цикла обработки
            } while (_isWork);
        }

        private void StartTick()
        {
            _tickStart = DateTime.UtcNow;
        }

        private void EndTick()
        {
            // Если цикл закончился раньше, чем должен начаться следующий цикл, то доспим малость
            var end = DateTime.UtcNow;
            int operationLengthInMs = Convert.ToInt32(end.Subtract(_tickStart).TotalMilliseconds);
            int msToSleep = HeartBeatCycleInMs - operationLengthInMs;
            if (msToSleep > 0)
                Thread.Sleep(TimeSpan.FromMilliseconds(msToSleep));
        }

        private void Collect()
        {
            var end = DateTime.UtcNow;
            var start = _startGarbageCollector.Ticks / TimeSpan.TicksPerMillisecond;
            var difference = (end.Ticks / TimeSpan.TicksPerMillisecond - start);

            if (difference >= 10000)
            {
                GC.Collect(2, GCCollectionMode.Forced, false);
                _startGarbageCollector = DateTime.UtcNow;
            }
        }
    }
}
