namespace ServiceWorker.Abstract
{
    public interface IWorker
    {
        int HeartBeatCycleInMs { get; set; }
        void Start();
        void Stop();
    }
}
