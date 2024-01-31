namespace ServiceWorker.Infrastructure
{
    using GlobalSpace.Common.Guardly;
    using ServiceWorker.Abstract;

    public class ServiceModelFacade : IServiceModelFacade
    {
        private bool _start;
        private bool _executing;

        private readonly ICommandService _command;

        public ServiceModelFacade(ICommandService command)
        {
            Guard.Argument(() => command, Is.NotNull);

            _command = command;
            _start = true;
        }

        public void Start()
        {
            _start = true;
        }

        public void Stop()
        {
            _start = false;
        }

        public void DoWork()
        {
            if (!_start) return;
            if (_executing) return;

            _executing = true;
            try
            {
                _command.Run();
            }
            finally
            {
                _executing = false;
            }
        }
    }
}
