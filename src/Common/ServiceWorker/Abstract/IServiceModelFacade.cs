namespace ServiceWorker.Abstract
{
    public interface IServiceModelFacade
    {
        void Start();
        void Stop();
        void DoWork();
    }
}
