namespace Microservices.Common.Infrastructure.Model
{
    public class ServiceIdentity
    {
        public ServiceIdentity(string guid)
        {
            Guid = guid;
        }

        public string Guid { get; }
    }
}
