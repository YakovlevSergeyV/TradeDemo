namespace Microservices.Common.WindowsService
{
    using System.ServiceProcess;
    using Microsoft.AspNetCore.Hosting;

    public static class WebHostServiceExtensions
    {
        public static void RunAsCustomService(this IWebHost host)
        {
            var webHostService = new CustomWebHostService(host);
            ServiceBase.Run(webHostService);
        }
    }
}
