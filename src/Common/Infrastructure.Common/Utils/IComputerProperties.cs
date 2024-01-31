
namespace Infrastructure.Common.Utils
{
    public sealed class ComputerInfo
    {
        public ComputerInfo(string domainName, string hostName, string macAddress)
        {
            DomainName = domainName;
            HostName = hostName;
            MacAddress = macAddress;
        }

        public string DomainName { get; }
        public string HostName { get; }
        public string MacAddress { get; }
    }

    public interface IComputerProperties
    {
        ComputerInfo GetInfo();
    }
}
