namespace Infrastructure.Common.Utils
{
    using System.Linq;
    using System.Net.NetworkInformation;

    public sealed class ComputerProperties : IComputerProperties
    {
        public ComputerInfo GetInfo()
        {
            var computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            var macAddress = string.Empty;
            foreach (var adapter in
                from adapter in nics
                where macAddress == string.Empty
                let properties = adapter.GetIPProperties()
                select adapter)
            {
                macAddress = adapter.GetPhysicalAddress().ToString();
                break;
            }

            return new ComputerInfo(computerProperties.DomainName, computerProperties.HostName, string.Empty);
        }
    }
}
